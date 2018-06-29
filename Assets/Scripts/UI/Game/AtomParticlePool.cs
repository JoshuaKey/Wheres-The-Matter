using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomParticlePool : MonoBehaviour {

    [SerializeField] AtomParticle atomParticlePrefab;
    [SerializeField] RectTransform particleDestination;

    [SerializeField] int particleSize;

    AtomParticle[] atomParticles;
    int currParticle;

    private void Start() {
        atomParticles = new AtomParticle[particleSize];
        for (int i = 0; i < particleSize; i++) {
            atomParticles[i] = Instantiate(atomParticlePrefab);
            atomParticles[i].gameObject.SetActive(false);
            atomParticles[i].transform.SetParent(this.transform);
        }
        Disable();
    }

    // Update is called once per frame
    void Update () {
		if(currParticle == 0) {
            Disable();
            return;
        }

        for(int i = 0; i < currParticle; i++) {
            var particle = atomParticles[i].rect;

            Vector3 distance = particleDestination.position - particle.position;
            if (distance.sqrMagnitude < 900f) { // 30 squared
                // Fade?
                // Another Effect?

                Game.Instance.Absorb(atomParticles[i].atom, atomParticles[i].amo);
                atomParticles[i].gameObject.SetActive(false);
                Swap(i, --currParticle);
                i--;
            } else {
                var position = Vector3.Lerp(particle.position, particleDestination.position, Time.deltaTime);

                particle.position = position;
            }
        }
	}

    public void AddParticle(Atom a, int amo, Vector2 pos) {
        if(currParticle == 0) { Enable(); }
        if(currParticle == atomParticles.Length) {
            particleSize = atomParticles.Length + 100;
            AtomParticle[] tempArray = new AtomParticle[particleSize];
            for(int i = 0; i < atomParticles.Length; i++) {
                tempArray[i] = atomParticles[i];
            }
            for (int i = atomParticles.Length; i < particleSize; i++) {
                tempArray[i] = Instantiate(atomParticlePrefab);
                tempArray[i].gameObject.SetActive(false);
                tempArray[i].transform.SetParent(this.transform);
            }
            atomParticles = tempArray;
        }
        atomParticles[currParticle++].Setup(a, amo, pos);
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
}
