using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AtomParticlePool : MonoBehaviour {

    [SerializeField] AtomParticle atomParticlePrefab;
    [SerializeField] Cursor cursor;

    [SerializeField] int particleMaxAmo;
    [SerializeField] float particleCollectDistance;

    AtomParticle[] atomParticles;
    [SerializeField] int currParticle;

    private void Start() {
        atomParticles = new AtomParticle[particleMaxAmo];
        for (int i = 0; i < particleMaxAmo; i++) {
            atomParticles[i] = InstantiateParticle();
        }
        Disable();

        Game.Instance.playerData.OnCollectRadiusChange += ChangeRadius;
        ChangeRadius(Game.Instance.playerData.GetValue(PlayerData.UpgradeType.Collect_Radius));
    }

    private AtomParticle InstantiateParticle() {
        var particle = Instantiate(atomParticlePrefab);

        particle.transform.SetParent(this.transform, false);
        
        particle.gameObject.SetActive(false);

        return particle;
    }

    public void Story() {
        switch (Game.Instance.story.GetChapter()) {
            case 3:
                this.gameObject.SetActive(true);
                this.enabled = false;
                StartCoroutine(AtomExplosion());
                break;
            default:
                StopCoroutine(AtomExplosion());
                this.enabled = true;
                this.gameObject.SetActive(true);
                for (int i = 0; i < 50 && i < particleMaxAmo; i++) {
                    atomParticles[i].gameObject.SetActive(false);
                }
                break;
        }
    }
    public IEnumerator AtomExplosion() {
        Atom hydrogen = Game.Instance.gameData.FindAtom(1);
        Vector3[] dir = new Vector3[50];
        for(int i = 0; i < 50; i++) {
            float angle = Random.Range(0, 360f);

            dir[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), .0f) * Random.Range(100, 500);

            atomParticles[i].Setup(hydrogen, 0, cursor.transform.position);
        }

        float endTime = Time.time + 5f;
        while(Time.time < endTime) {
            //float t = 1 - (endTime - Time.time) / 5f;

            for (int i = 0; i < 50; i++) {
                var pos = atomParticles[i].transform.position;
                atomParticles[i].UpdateParticle(pos + dir[i], 0f, 15f, 2f);
            }
            yield return null;
        }

        this.gameObject.SetActive(false);
        this.enabled = true;
    }

    // Update is called once per frame
    void Update () {
		if(currParticle == 0) {
            Disable();
            return;
        }

        for(int i = 0; i < currParticle; i++) {
            if (atomParticles[i].UpdateParticle(cursor.transform.position, particleCollectDistance, 15f, 1f)) {
                Game.Instance.Absorb(atomParticles[i].atom, atomParticles[i].amo);
                atomParticles[i].gameObject.SetActive(false);
                Swap(i, --currParticle);
                i--;
            }
        }
	}

    public void AddParticle(Atom a, int amo, Vector2 pos) {
        if(currParticle == 0) { Enable(); }

        if(currParticle > 100) { // Limit new Particles...
            //for (int i = 0; i < atomParticles.Length; i++) { // Search for existing and add onto
            //    if(atomParticles[i].atom == a) {
            //        atomParticles[i].amo += amo; // Linq?
            //        return;
            //    }
            //}
            var items = atomParticles.Where((x) => {
                return x.atom == a;
            });
            for(int i = 0; i < items.Count(); ) {
                items.ElementAt(i).amo += amo;
                return;
            }
        }
        if (currParticle == atomParticles.Length) { // Otherwise, create new Particle
            Reallocate();
        }
        atomParticles[currParticle++].Setup(a, amo, pos);
    }
    private void Reallocate() {
        particleMaxAmo = atomParticles.Length + 100;
        AtomParticle[] tempArray = new AtomParticle[particleMaxAmo];
        for (int i = 0; i < atomParticles.Length; i++) {
            tempArray[i] = atomParticles[i];
        }
        for (int i = atomParticles.Length; i < particleMaxAmo; i++) {
            tempArray[i] = InstantiateParticle();
        }
        atomParticles = tempArray;
    }

    private void Swap(int a, int b) {
        AtomParticle temp = atomParticles[a];
        atomParticles[a] = atomParticles[b];
        atomParticles[b] = temp;
    }

    private void Enable() {
        this.gameObject.SetActive(true);
    }
    private void Disable() {
        this.gameObject.SetActive(false);
    }

    private void ChangeRadius(float radi) {
        // 923
        //particleCollectDistance = Mathf.Max(.5f, radi / 2) * 25;
        particleCollectDistance = Mathf.Max(.5f, radi / 2) * Screen.width / 35f;
        particleCollectDistance *= particleCollectDistance;
    }
}
