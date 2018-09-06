using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AtomCollector : MonoBehaviour {

    [Serializable]
    public struct AtomRatio {
        public Atom atom;
        public float ratio;
    }

    [Header("Atoms")]
    public int totalAtomAmo; // Total Max Amount of Atoms in this object
    public List<AtomRatio> atoms; // Ratios of Atoms

    [Header("Collect")]
    public float collectionPause = .2f;

    [Header("Life")]
    public bool hasLife = true;
    public Color aliveColor = Color.white;
    public Color deadColor = new Color(.0f, .0f, .0f, .0f);
    // Make several Waypoionts of color for certain objects? Like brown, gray, black, fade for Tree?

    [Header("Flash")]
    public bool canFlash = false;
    public float flashAlphaMultiple = .6f;

    [Header("Other")]
    [SerializeField] private List<AtomAmo> currAtoms = new List<AtomAmo>(); // Current atoms and their amount in  this Object
    [SerializeField] private int currAtomAmo;

    [SerializeField] private float nextCollectionTime;

    [SerializeField] private Color currColor;
    [SerializeField] private bool isFlashing = false;

    private new SpriteRenderer renderer;

    private void Start() {
        renderer = GetComponent<SpriteRenderer>();

        aliveColor = renderer.color;

        this.gameObject.layer = LayerMask.NameToLayer("AtomCollector");

        AtomAmo amo = new AtomAmo();
        float totalRatio = 0f;

        for (int i = 0; i < atoms.Count; i++) {
            amo.atom = atoms[i].atom;
            double ratio = atoms[i].ratio / 100d;
            amo.amo = (int)(ratio * totalAtomAmo);
            currAtoms.Add(amo);

            totalRatio += atoms[i].ratio;
        }
        if(100.0f - totalRatio > .001f || totalRatio > 100.0f) {
            print(name + " has Ratio of " + totalRatio);
        }

        currAtomAmo = totalAtomAmo;
        currColor = aliveColor;

        int tempAtomAmo = GetSum(currAtoms);
        if(tempAtomAmo > 0) {
            currAtomAmo = tempAtomAmo;
        }

        //if(tempAtomAmo != currAtomAmo) {
        //    print("Object " + name + " Total Amo (" + totalAtomAmo + ") doesnt equal Curr Amo (" + tempAtomAmo + 
        //        ")");
        //}
    }

    public void AddAtom(AtomRatio a) {
        atoms.Add(a);

        currAtoms.Clear();
        AtomAmo amo = new AtomAmo();
        for (int i = 0; i < atoms.Count; i++) {
            amo.atom = atoms[i].atom;
            double ratio = atoms[i].ratio / 100d;
            amo.amo = (int)(ratio * totalAtomAmo);
            currAtoms.Add(amo);
        }
    }
    public int GetSum(List<AtomAmo> amo) {
        int sum = 0;
        for(int i  = 0; i < amo.Count; i++) {
            sum += amo[i].amo;
        }
        return sum;
    }

    public List<AtomAmo> Absorb() {
        List<AtomAmo> atomsToBeAbsorbed = new List<AtomAmo>();
        if(Time.time < nextCollectionTime) { return atomsToBeAbsorbed; }

        PlayerData playerData = Game.Instance.playerData;
        AtomAmo amoToCollect = new AtomAmo();
        int atomCount = 0;
        float weightLimit = playerData.GetValue(PlayerData.UpgradeType.Collect_Weight);
        float speedRatio = playerData.GetValue(PlayerData.UpgradeType.Collect_Speed);
        int efficiencyAmo = (int)playerData.GetValue(PlayerData.UpgradeType.Collect_Efficiency);

        for (int i = 0; i < currAtoms.Count; i++) {
            AtomAmo atomAmo = currAtoms[i];
            if(atomAmo.amo <= 0) { continue; }

            AtomInfo info = Game.Instance.gameData.FindAtomInfo(atomAmo.atom.GetAtomicNumber());
            if (info.GetProtons() + info.GetNeutrons() > weightLimit) { continue; }

            int maxAmo = efficiencyAmo;/// info.GetProtons();
            if (maxAmo < 2) { maxAmo = 2; }

            int randAmo = UnityEngine.Random.Range(0, maxAmo);
            if(randAmo == 0) { continue; }

            amoToCollect.atom = atomAmo.atom;
            amoToCollect.amo = randAmo > atomAmo.amo ? atomAmo.amo : randAmo;
            
            atomsToBeAbsorbed.Add(amoToCollect);
            atomCount += amoToCollect.amo;

            if (hasLife) {
                atomAmo.amo -= amoToCollect.amo; // Because Dumb
                currAtomAmo -= amoToCollect.amo;
                currAtoms[i] = atomAmo;
            }
        }

        // Time
        if(atomCount != 0) {
            nextCollectionTime = Time.time + collectionPause * speedRatio;
        }

        // Life
        if (hasLife) {
            CheckLife();
        }

        // Flash
        if (canFlash && !isFlashing && atomCount != 0 && !IsDead()) {
            StartCoroutine(Flash());
        }

        return atomsToBeAbsorbed;
    }

    public void CheckLife() {
        if (!hasLife) { return; }

        if (IsDead()) {
            this.gameObject.SetActive(false);
            return;
        }

        float t = 1 - (float)currAtomAmo / totalAtomAmo;

        currColor = Color.Lerp(aliveColor, deadColor, t);
        if (renderer != null) {
            renderer.color = currColor;
        }
    }

    public bool IsDead() {
        return currAtomAmo <= 0f;
    }
    public bool CanBeCollected() {
        return Time.time > nextCollectionTime;
    }

    private IEnumerator Flash() {
        isFlashing = true;

        Color startColor = currColor;
        Color endColor = startColor;
        endColor.a *= flashAlphaMultiple;

        // Flash
        float startTime = Time.time;
        while (Time.time < startTime + collectionPause) {
            float t = (Time.time - startTime) / collectionPause;

            startColor = currColor;
            endColor = startColor;
            endColor.a *= flashAlphaMultiple;

            Color c = Color.Lerp(startColor, endColor, t);
            renderer.color = c;

            yield return null;
        }
        renderer.color = endColor;

        // Normal
        startTime = Time.time;
        while (Time.time < startTime + collectionPause) {
            float t = (Time.time - startTime) / collectionPause;

            startColor = currColor;
            endColor = startColor;
            endColor.a *= flashAlphaMultiple;

            Color c = Color.Lerp(endColor, startColor, t);
            renderer.color = c;

            yield return null;
        }
        renderer.color = startColor;

        isFlashing = false;
    }

    private void OnDisable() {
        isFlashing = false;
        //renderer.color = currColor;
    }

}
