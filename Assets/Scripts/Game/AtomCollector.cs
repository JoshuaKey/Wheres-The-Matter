using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomCollector : MonoBehaviour {

    [Serializable]
    public struct AtomRatio {
        public Atom atom;
        public float ratio;
    }

    [Header("Collection")]
    public List<AtomRatio> atoms;
    public float collectionPause = .2f;

    private float nextCollectionTime;

    [Header("Life")]
    public bool hasLife = true;
    public float life = 100.0f;
    public float maxLife = 100.0f; //Amount of Atoms in Object * 100. An atom is .001
    public Color aliveColor = Color.white;
    public Color deadColor = new Color(.0f, .0f, .0f, .0f);
    // Make several Waypoionts of color for certain objects? Like brown, gray, black, fade for Tree?

    [Header("Flash")]
    public bool canFlash = false;
    public float flashAlphaMultiple = .6f;

    private bool isFlashing = false;

    private new SpriteRenderer renderer;

    private void Start() {
        renderer = GetComponent<SpriteRenderer>();

        this.gameObject.layer = LayerMask.NameToLayer("AtomCollector");
     }

    public List<AtomAmo> Absorb() {
        List<AtomAmo> atomsToBeAbsorbed = new List<AtomAmo>();
        if(Time.time < nextCollectionTime) { return atomsToBeAbsorbed; }

        var playerData = Game.Instance.playerData;
        int atomCount = 0;

        for (int i = 0; i < atoms.Count; i++) {
            AtomRatio atomRatio = atoms[i];

            float ratio = UnityEngine.Random.Range(.0f, 100.0f) * playerData.GetAtomCollectorEfficiency();
            if(ratio < atomRatio.ratio) {
                AtomAmo atomAmo = new AtomAmo();
                atomAmo.atom = atomRatio.atom;
                atomAmo.amo = 1;

                atomsToBeAbsorbed.Add(atomAmo);

                atomCount += atomAmo.amo;
            }
        }
       
        // Time
        nextCollectionTime = Time.time + collectionPause * playerData.GetAtomCollectorSpeed();
        //print("Time: " + Time.time + " NExt: " + nextCollectionTime);

        // Scale
        if (canFlash && !isFlashing && atomCount != 0) {
            StartCoroutine(Flash());
        }

        // Life
        LoseLife(atomCount / 100f);

        return atomsToBeAbsorbed;
    }

    public void LoseLife(float damage) {
        if (!hasLife) { return; }

        life -= damage;
        Color c = deadColor;

        if (IsDead()) {
            life = .0f;
            this.gameObject.SetActive(false);
        } else {

            float t = 1 - life / maxLife;
            c =  Color.Lerp(aliveColor, deadColor, t);
        }

        if (renderer != null) {
            renderer.color = c;
        }
    }
    public bool IsDead() {
        return life <= 0f;
    }

    private IEnumerator Flash() {
        isFlashing = true;

        Color startColor = renderer.color;
        Color endColor = startColor;
        endColor.a *= flashAlphaMultiple;

        // Flash
        float startTime = Time.time;
        while (Time.time < startTime + collectionPause) {
            float t = (Time.time - startTime) / collectionPause;

            Color c = Color.Lerp(startColor, endColor, t);
            renderer.color = c;

            yield return null;
        }
        renderer.color = endColor;

        // Normal
        startTime = Time.time;
        while (Time.time < startTime + collectionPause) {
            float t = (Time.time - startTime) / collectionPause;

            Color c = Color.Lerp(endColor, startColor, t);
            renderer.color = c;

            yield return null;
        }
        renderer.color = startColor;

        isFlashing = false;
    }

    private void OnDisable() {
        isFlashing = false;
    }

}
