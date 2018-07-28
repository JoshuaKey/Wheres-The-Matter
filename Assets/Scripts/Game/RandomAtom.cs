using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAtom : MonoBehaviour {

    [SerializeField] public AtomCollector atomCollector;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 88; i++) {
            if (UnityEngine.Random.value < .1f) {
                AtomCollector.AtomRatio ratio = new AtomCollector.AtomRatio();
                ratio.atom = Game.Instance.gameData.FindAtom(i + 1);
                ratio.ratio = UnityEngine.Random.value * 100;

                atomCollector.AddAtom(ratio);
                break;
            }
        }
    }
	
}
