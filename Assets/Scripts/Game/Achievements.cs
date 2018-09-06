using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Game.Instance.gameData.OnAtomAdd += CheckAtomAdd;
        Game.Instance.gameData.OnAtomDiscover += CheckAtomDiscovery;
        // Don't Forget SECRET UI achievement...
        // Don't forget MASTERY achievement 

        // Make a sonic Screwdriver?
        // Sometimes Log doesnt Work
        // Exclamation Points
	}

    public void CheckUpgrade() {
        // Check Time Unlock        (Doctor Who)* In Atom Discovery...

        // Check all Upgrades Max   (Overload!)
        // Check Spped Max          (MAXIMUM OVERDRIVE!)
        // Check Efficiency Max     (You get an Atom, You get and Atom, Everyone gets an Atom!)
        // Chekc Radius Max         (All the Atoms!)
        // Check Weight Max         (Ain't she thicc..)
        // Check Stability Max      (
        // Check Accleration Max    (To Infinity and Beyond!)
        // Check Time Max           (Timey Whimey)
    }
    public void CheckAtomDiscovery(Atom a, float amo) {
        Game.Instance.logSystem.Log("Discovered " + a.GetName());

        // Check Hydrogen           (Participation Award)
        // Check All Real Elements  (
        // Check All Noble Gases    (Back down to Earth)
        // Check all Metals         (Smithy)
        // Check All ALL Elements   (Doctor Scientist)
        // Check Helium             (It's a big world)
        // Check New Element        (Ahead of the Curb)

    }
    public void CheckAtomAdd(Atom a, float amo) {
        CheckAtomMilestone(a, amo);

        // Check for Max        (Max Capacity)
        // Check for full Max   (MAX CAPACITY)
    }
    public void CheckAtomUse(Atom a, float amo) {

    }
    public void CheckCraftableAdd(Craftable c, int amo) {
        // Made every Craftable?
    }
    public void CheckSell(float money) {
        // Money Milestones
        // Win
        // Win Time < 2 hours?  (Ain't nobody got time for dat!)
    }


    private void CheckAtomMilestone(Atom a, float amo) {
        AtomData data = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
        int currAmo = data.GetCurrAmo();
        int prevAmo = currAmo - (int)amo;

        if(currAmo == int.MaxValue) {
            Game.Instance.logSystem.Log(a.GetName() + ": MAX");
            return;
        }

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
