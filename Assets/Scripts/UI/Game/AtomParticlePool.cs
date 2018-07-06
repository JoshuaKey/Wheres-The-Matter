using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomParticlePool : MonoBehaviour {

    [SerializeField] AtomParticle atomParticlePrefab;
    [SerializeField] Transform particleDestination;

    [SerializeField] int particleMaxAmo;
    [SerializeField] float particleCollectDistance;

    AtomParticle[] atomParticles;
    int currParticle;

    private void Start() {
        atomParticles = new AtomParticle[particleMaxAmo];
        for (int i = 0; i < particleMaxAmo; i++) {
            atomParticles[i] = InstantiateParticle();
        }
        Disable();

        Game.Instance.playerData.OnCollectRadiusChange += ChangeRadius;
        ChangeRadius(Game.Instance.playerData.GetAtomCollectorRadius());
    }

    private AtomParticle InstantiateParticle() {
        var particle = Instantiate(atomParticlePrefab);

        particle.transform.SetParent(this.transform, false);
        
        particle.gameObject.SetActive(false);

        return particle;
    }

    // Update is called once per frame
    void Update () {
		if(currParticle == 0) {
            Disable();
            return;
        }

        for(int i = 0; i < currParticle; i++) {
            if (atomParticles[i].UpdateParticle(particleDestination.position, particleCollectDistance, 5f, 1f)) {
                Game.Instance.Absorb(atomParticles[i].atom, atomParticles[i].amo);
                atomParticles[i].gameObject.SetActive(false);
                Swap(i, --currParticle);
                i--;
            }
        }
	}

    public void AddParticle(Atom a, int amo, Vector2 pos) {
        if(currParticle == 0) { Enable(); }
        if(currParticle == atomParticles.Length) {
            particleMaxAmo = atomParticles.Length + 100;
            AtomParticle[] tempArray = new AtomParticle[particleMaxAmo];
            for(int i = 0; i < atomParticles.Length; i++) {
                tempArray[i] = atomParticles[i];
            }
            for (int i = atomParticles.Length; i < particleMaxAmo; i++) {
                tempArray[i] = InstantiateParticle();
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

    private void ChangeRadius(float radi) {
        particleCollectDistance = Mathf.Max(.5f, radi / 2) * 25;
        particleCollectDistance *= particleCollectDistance;
    }
}
