using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAtom : MonoBehaviour {

    [SerializeField] public AtomCollector atomCollector;
    [SerializeField] public SpriteRenderer spriteRenderer;

    [SerializeField] private Atom[] randomAtoms;
    [SerializeField] private Color[] randomColors;
    [SerializeField] private float amo;

	// Use this for initialization
	void Start () {
        int index = Random.Range(0, randomAtoms.Length);

        AtomCollector.AtomRatio atomAmo = new AtomCollector.AtomRatio();
        atomAmo.atom = randomAtoms[index];
        atomAmo.ratio = amo;

        atomCollector = atomCollector ?? GetComponent<AtomCollector>();
        spriteRenderer = spriteRenderer ?? GetComponent<SpriteRenderer>();

        if(atomCollector == null) {
            print("Could not find Atom Collector on " + name);
            return;
        }

        if (randomColors.Length > index) {
            spriteRenderer.color = randomColors[index];
            atomCollector.aliveColor = randomColors[index];
        }


        atomCollector.AddAtom(atomAmo);
    }
	
}
