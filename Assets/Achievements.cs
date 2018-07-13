using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Game.Instance.gameData.OnAtomAdd += CheckAtomMilestone;
        Game.Instance.gameData.OnAtomDiscover += (x, y) => {
            Game.Instance.logSystem.Log("Discovered " + x.GetName());
        };
	}

    public void CheckAtomMilestone(Atom a, float amo) {
        AtomData data = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
        int currAmo = data.GetCurrAmo();
        int prevAmo = currAmo - (int)amo;

        bool hasSurpassed = false;
        for (int i = 1000000000; i > 1; i /= 10) {
            if(CheckAtomAmo(currAmo, prevAmo, i, ref hasSurpassed)) {
                Game.Instance.logSystem.Log(a.GetName() + ": " + i);
                break;
            }
            if (hasSurpassed) {
                break;
            }
        }
    }
    private bool CheckAtomAmo(int currAmo, int prevAmo, int compareAmo, ref bool hasSurpassed) {
        if (currAmo >= compareAmo) { // A billion
            hasSurpassed = true;
            if (prevAmo < compareAmo) {
                return true;
            }
        }
        return false;
    }
}
