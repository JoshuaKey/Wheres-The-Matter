using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData  {
    // Player Data is responsible for holding and calculating data relevant to the player. Their current progression

    public struct AtomCollision {
        public Atom targetAtom; // What Atom will we be creating
        public int amo; // Max Amount
        public float success; // How Likely it is to succeed
        public float stability; // How likely it is to be stable
    }
    public struct AtomCollisionResult {
        public List<AtomAmo> atomsProduced;
        public List<AtomAmo> atomsUsed;
    }
    public struct CraftResult {
        public List<AtomAmo> atomsUsed;
        public int amountCreated;
    }

    [SerializeField] private float collectRadius = .3f; // From .3 to Inifinite
    [SerializeField] private float collectSpeed = 0f; // From 1 -  0
    [SerializeField] private float collectEfficiency = 1f; // From 1 to Infinite
    [SerializeField] private float collectWeight = 1f; // From 1 - 300ish Neutrons + Protons

    [SerializeField] private float particleSpeed = 1.5f; // From 1.5 to  100
    [SerializeField] private float particleStability = .6f;

    // Add weight
    // All values are floats
    // What is the max, what is the min.

    private AtomAmo collectRadiusCost;
    private AtomAmo collectSpeedCost;
    private AtomAmo collectEfficiencyCost;
    private AtomAmo collectWeightCost;
            
    private AtomAmo particleSpeedCost;
    private AtomAmo particleStablizationCost;

    private float money;

    private Dictionary<Craftable, int> craftables = new Dictionary<Craftable, int>();

    public void Init() {
        var gameData = Game.Instance.gameData;

        Atom hydrogen = gameData.FindAtom(1);

        collectRadiusCost.atom = hydrogen;
        collectRadiusCost.amo = 50;

        collectSpeedCost.atom = hydrogen;
        collectSpeedCost.amo = 50;

        collectEfficiencyCost.atom = hydrogen;
        collectEfficiencyCost.amo = 50;

        collectWeightCost.atom = hydrogen;
        collectWeightCost.amo = 50;

        particleSpeedCost.atom = hydrogen;
        particleSpeedCost.amo = 50;

        particleStablizationCost.atom = hydrogen;
        particleStablizationCost.amo = 50;
    }

    public AtomCollision EstimateCombine(Atom a, Atom b, int aAmo, int bAmo) {
        AtomCollision info = new AtomCollision();

        AtomInfo aInfo = Game.Instance.gameData.FindAtomInfo(a.GetAtomicNumber());
        AtomInfo bInfo = Game.Instance.gameData.FindAtomInfo(b.GetAtomicNumber());
        AtomData aData = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
        AtomData bData = Game.Instance.gameData.FindAtomData(b.GetAtomicNumber());

        aAmo = Mathf.Min(aAmo, aData.GetCurrAmo());
        bAmo = Mathf.Min(bAmo, bData.GetCurrAmo());

        int maxProtons = aInfo.GetProtons() + bInfo.GetProtons(); 
        int totalANeutrons = aAmo * aInfo.GetNeutrons();
        int totalBNeutrons = bAmo * bInfo.GetNeutrons();

        if (Game.Instance.gameData.GetAtomAmount() >= maxProtons) {
            info.targetAtom = Game.Instance.gameData.FindAtom(maxProtons); // Should Correlate to Atomic number
        } //else {
        //    info.targetAtom = Game.Instance.gameData.GetUnknown(); // New Element
        //}
        AtomInfo targetInfo = Game.Instance.gameData.FindAtomInfo(maxProtons);

        int minProtons = Mathf.Min(aAmo, bAmo); // Min of Atoms
        int minNeutrons = (totalANeutrons + totalBNeutrons) / targetInfo.GetNeutrons();
        info.amo = Mathf.Min(minProtons, minNeutrons);

        // 119 / 10 -> 11.9% Speed
        // 1 - (11.9 / 12)  -> .008333 About 1 / 12
        // 
        // At 10% speed of light, Titanium -> Berkelium produces an estimated 1 / 1 billion atoms of Element 119
        float estimatedSpeed = maxProtons / 10f;
        info.success = Mathf.Clamp01(1 - estimatedSpeed / particleSpeed);

        // Apparently the half life of atoms can be changed by Time Dialation, Gravity, and External Radiation
        // There is no known way to accurately predict the half-life of atoms. There are two many variables
        // How about this, we place radioactive atoms near a black hole. Depending on the strength and randomness, the atom's half-life may be extended and stored indefinetly.

        // Our goal is to get that between 525600 1 year and 525600000 1000 years
        //  However if we have a value of 0.00085
        // That requires a multiple of 618,352,941 Just for the min

        //100*sqrt(1-((x*x)/(100*100))); Percentage of Speed -> Time Ratio
        //1 / sqrt(1-((x*x)/(100*100))); Percentage of Speed -> Time Multiple
        if (targetInfo.GetHalfLife() == 0f) {
            info.stability = 1.0f;
        } else {
            float halfLife = targetInfo.GetHalfLife();
            float multiple = 1 / Mathf.Sqrt(1 - (particleStability * particleStability) / (100 * 100));
            info.stability = AtomInfo.GetStability(halfLife * multiple * multiple);
        }

        return info;
    }
    public AtomCollisionResult ProduceCombine(Atom a, Atom b, int aAmo, int bAmo) {
        AtomCollision info = EstimateCombine(a, b, aAmo, bAmo);

        AtomCollisionResult result = new AtomCollisionResult();
        result.atomsProduced = new List<AtomAmo>();
        result.atomsUsed = new List<AtomAmo>();

        //AtomInfo aInfo = Game.Instance.gameData.FindAtomInfo(a.GetAtomicNumber());
        //AtomInfo bInfo = Game.Instance.gameData.FindAtomInfo(b.GetAtomicNumber());
        ////AtomData aData = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
        ////AtomData bData = Game.Instance.gameData.FindAtomData(b.GetAtomicNumber());

        //int maxProtons = aInfo.GetProtons() + bInfo.GetProtons();
        //int minAmo = Mathf.Min(aAmo, bAmo); // Min of Atoms
        //int totalNeutrons = aAmo * aInfo.GetNeutrons() + bAmo* bInfo.GetNeutrons();
        //int totalProtons = aAmo * aInfo.GetProtons() + bAmo * bInfo.GetProtons();

        //for (int i = maxProtons; i > 1; i--) {
        //if(totalNeutrons <= 0 || totalProtons <= 0f) { break; }

        //Atom target = Game.Instance.gameData.FindAtom(maxProtons);
        Atom target = info.targetAtom;
        //if(target == a || target == b) { break; }

        //AtomInfo targetInfo = Game.Instance.gameData.FindAtomInfo(maxProtons);
        //AtomInfo targetInfo = Game.Instance.gameData.FindAtomInfo(target.GetAtomicNumber());

        //int minNeutrons = totalNeutrons / targetInfo.GetNeutrons();
        //int maxAmo = Mathf.Min(minAmo, minNeutrons);
        int maxAmo = info.amo;
        ////if(maxAmo == 0) { continue; }

        //float estimatedSpeed = maxProtons / 10f;
        //float successChance = Mathf.Clamp01(1 - estimatedSpeed / particleSpeed);
        float successChance = info.success;
            //if(successChance == 0f) { continue; }

            //int produced = (int)(maxAmo * successChance);
            //totalNeutrons -= targetInfo.GetNeutrons() * produced;
            //totalProtons -= targetInfo.GetProtons() * produced;

            float stabilityChance = 1.0f;
        //if (targetInfo.GetHalfLife() != 0f) {
        //    float halfLife = targetInfo.GetHalfLife();
        //    float multiple = 1 / Mathf.Sqrt(1 - (particleStability * particleStability) / (100 * 100));
        //    stabilityChance = AtomInfo.GetStability(halfLife * multiple * multiple);
        //}
        stabilityChance = info.stability;
        //if (stabilityChance == 0f) { continue; }

        int produced = (int)(maxAmo * successChance);
        //totalNeutrons -= targetInfo.GetNeutrons() * produced;
        //totalProtons -= targetInfo.GetProtons() * produced;

        int stabilized = (int)(produced * stabilityChance);

            AtomAmo atomAmo = new AtomAmo();
            atomAmo.amo = stabilized;
            atomAmo.atom = target;
            result.atomsProduced.Add(atomAmo);

            Game.Instance.Absorb(target, stabilized); 
        //}

        AtomAmo atomAUsed = new AtomAmo();
        atomAUsed.atom = a;
        //atomAUsed.amo = minAmo;
        atomAUsed.amo = maxAmo;
        result.atomsUsed.Add(atomAUsed);

        AtomAmo atomBUsed = new AtomAmo();
        atomBUsed.atom = b;
        //atomBUsed.amo = minAmo;
        atomBUsed.amo = maxAmo;
        result.atomsUsed.Add(atomBUsed);

        //Game.Instance.Use(a, minAmo);
        //Game.Instance.Use(b, minAmo);
        Game.Instance.Use(a, maxAmo);
        Game.Instance.Use(b, maxAmo);

        // Tehcnincally if we fail to combine, theres no reason to create lesser atoms...

        return result;
    }

    public AtomCollision EstimateSplit(Atom a, int aAmo) {
        AtomCollision info = new AtomCollision();

        if(a.GetAtomicNumber() == 1) { // Hydrogen, Can Not Split...
            return info;
        }

        AtomInfo aInfo = Game.Instance.gameData.FindAtomInfo(a.GetAtomicNumber());

        int halfProtons = aInfo.GetProtons() / 2;
        int totalNeutrons = aInfo.GetNeutrons() * aAmo;

        // Target
        info.targetAtom = Game.Instance.gameData.FindAtom(halfProtons); // Should Correlate to Atomic number
        AtomInfo targetInfo = Game.Instance.gameData.FindAtomInfo(halfProtons);

        // Max Amount to Be Made
        if(info.targetAtom.GetAtomicNumber() == 1) { // Hydrogen
            info.amo = aInfo.GetProtons() * aAmo / targetInfo.GetProtons(); // Proton Based
        } else {
            info.amo = totalNeutrons / targetInfo.GetNeutrons(); // Neutron Based
        }
        
        // Success
        float estimatedSpeed = aInfo.GetProtons() / 10f;
        info.success = Mathf.Clamp01(1 - estimatedSpeed / particleSpeed);

         // Stability
        if (targetInfo.GetHalfLife() == 0f) {
            info.stability = 1.0f;
        } else {
            float halfLife = targetInfo.GetHalfLife();
            float multiple = 1 / Mathf.Sqrt(1 - (particleStability * particleStability) / (100 * 100));
            info.stability = AtomInfo.GetStability(halfLife * multiple * multiple);
        }

        return info;
    }
    public AtomCollisionResult ProduceSplit(Atom a, int aAmo) {
        AtomCollision info = EstimateSplit(a, aAmo);

        AtomData aData = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
        aAmo = Mathf.Min(aAmo, aData.GetCurrAmo());

        AtomCollisionResult result = new AtomCollisionResult();
        result.atomsProduced = new List<AtomAmo>();
        result.atomsUsed = new List<AtomAmo>();

        Atom target = info.targetAtom;
        //AtomInfo targetInfo = Game.Instance.gameData.FindAtomInfo(target.GetAtomicNumber());

        // Split
        int produced = (int)(info.amo * info.success); // Random Loss

        // Stabilize
        int stabilized = (int)(produced * info.stability);

        // Result
        AtomAmo atomAmo = new AtomAmo();
        atomAmo.amo = stabilized;
        atomAmo.atom = target;
        result.atomsProduced.Add(atomAmo);
        Game.Instance.Absorb(target, stabilized);

        // Used
        AtomAmo atomAUsed = new AtomAmo();
        atomAUsed.atom = a;
        atomAUsed.amo = aAmo;
        result.atomsUsed.Add(atomAUsed);
        Game.Instance.Use(a, aAmo);

        return result;
    }

    // Estimate Atom

    public bool CanCraft(Craftable c) {
        int amount = 1;

        var atoms = c.GetAtomsForProduction();
        for (int i = 0; i < atoms.Length; i++) { // Find Minumum
            var atomAmo = atoms[i];

            AtomData data = Game.Instance.gameData.FindAtomData(atomAmo.atom.GetAtomicNumber());

            int needed = atomAmo.amo * amount;
            if (data.GetCurrAmo() < needed) {
                amount = data.GetCurrAmo() / atomAmo.amo;
            }
        }

        return amount != 0;
    }
    public CraftResult Craft(Craftable c, int amount) {
        CraftResult result = new CraftResult();
        result.atomsUsed = new List<AtomAmo>();

        var atoms = c.GetAtomsForProduction();
        for(int i = 0; i < atoms.Length; i++) { // Find Minumum
            var atomAmo = atoms[i];

            AtomData data = Game.Instance.gameData.FindAtomData(atomAmo.atom.GetAtomicNumber());

            int needed = atomAmo.amo * amount;
            if(data.GetCurrAmo() < needed) {
                amount = data.GetCurrAmo() / atomAmo.amo;               
            }
        }

        for (int i = 0; i < atoms.Length; i++) {
            var atomAmo = atoms[i];

            AtomData data = Game.Instance.gameData.FindAtomData(atomAmo.atom.GetAtomicNumber());

            int needed = atomAmo.amo * amount;
            data.Lose(needed);

            AtomAmo atomUsed = new AtomAmo();
            atomUsed.atom = atomAmo.atom;
            atomUsed.amo = needed;
            result.atomsUsed.Add(atomUsed);
        }
        result.amountCreated = amount;

        int amoOfCraftables;
        if(!craftables.TryGetValue(c, out amoOfCraftables)) {
            craftables[c] = amount;
        } else {
            craftables[c] = amoOfCraftables + amount;
        }

        return result;
    }

    public void Sell(Craftable c, int amo) {
        int amoOfCraftables;
        if (craftables.TryGetValue(c, out amoOfCraftables)) {
            craftables[c] = amoOfCraftables - amo;
            money += c.GetPrice() * amo;
        } 
    }

    public bool UpgradeAtomCollectorRadius() {
        bool success = false;
        var gameData = Game.Instance.gameData;

        AtomData data = gameData.FindAtomData(collectRadiusCost.atom.GetAtomicNumber());

        if(data.GetCurrAmo() >= collectRadiusCost.amo) {
            data.Lose(collectRadiusCost.amo);

            // Update CollectRadius Cost
            collectRadiusCost.amo *= 2;

            // Increment Collect Radius
            collectRadius += .1f;

            success = true;
        }

        return success;
    }
    public bool UpgradeAtomCollectorSpeed() {
        bool success = false;
        var gameData = Game.Instance.gameData;

        AtomData data = gameData.FindAtomData(collectSpeedCost.atom.GetAtomicNumber());

        if (data.GetCurrAmo() >= collectSpeedCost.amo) {
            data.Lose(collectSpeedCost.amo);

            // Update CollectRadius Cost
            collectSpeedCost.amo *= 2;

            // Increment Collect Radius
            collectSpeed -= .01f;

            success = true;
        }

        return success;
    }
    public bool UpgradeAtomCollectorEfficiency() {
        bool success = false;
        var gameData = Game.Instance.gameData;

        AtomData data = gameData.FindAtomData(collectEfficiencyCost.atom.GetAtomicNumber());

        if (data.GetCurrAmo() >= collectEfficiencyCost.amo) {
            data.Lose(collectEfficiencyCost.amo);

            // Update CollectRadius Cost
            collectEfficiencyCost.amo *= 2;

            // Increment Collect Radius
            collectEfficiency += .01f;

            success = true;
        }

        return success;
    }
    public bool UpgradeAtomCollectorWeight() {
        bool success = false;
        var gameData = Game.Instance.gameData;

        AtomData data = gameData.FindAtomData(collectEfficiencyCost.atom.GetAtomicNumber());

        if (data.GetCurrAmo() >= collectEfficiencyCost.amo) {
            data.Lose(collectEfficiencyCost.amo);

            // Update CollectRadius Cost
            collectWeightCost.amo *= 2;

            // Increment Collect Radius
            collectWeight += 2f;

            success = true;
        }

        return success;
    }
    public bool UpgradeParticleSpeed() {
        bool success = false;
        var gameData = Game.Instance.gameData;

        AtomData data = gameData.FindAtomData(particleSpeedCost.atom.GetAtomicNumber());

        if (data.GetCurrAmo() >= particleSpeedCost.amo) {
            data.Lose(particleSpeedCost.amo);

            // Update CollectRadius Cost
            particleSpeedCost.amo *= 2;

            // Increment Collect Radius
            particleSpeed += 1f;

            success = true;
        }

        return success;
    }
    public bool UpgradeParticleStabilization() {
        bool success = false;
        var gameData = Game.Instance.gameData;

        AtomData data = gameData.FindAtomData(particleStablizationCost.atom.GetAtomicNumber());

        if (data.GetCurrAmo() >= particleStablizationCost.amo) {
            data.Lose(particleStablizationCost.amo);

            // Update CollectRadius Cost
            particleStablizationCost.amo *= 2;

            // Increment Collect Radius
            particleStability += .01f;
            // Logarithmic increment

            success = true;
        }

        return success;
    }

    public float GetAtomCollectorRadius() { return collectRadius; }
    public float GetAtomCollectorSpeed() { return collectSpeed; }
    public float GetAtomCollectorEfficiency() { return collectEfficiency; }
    public float GetAtomCollectorWeight() { return collectWeight; }
    public float GetParticleSpeed() { return particleSpeed; }
    public float GetParticleStabilization() { return particleStability; }

    public int GetCraftableAmount(Craftable c) {
        int amo = 0;
        if(craftables.TryGetValue(c, out amo)) {
            return amo;
        }
        return amo;
    }
    public float GetMoney() { return money; }

    public AtomAmo GetAtomCollectorRadiusCost() { return collectRadiusCost; }
    public AtomAmo GetAtomCollectorSpeedCost() { return collectSpeedCost; }
    public AtomAmo GetAtomCollectorEfficiencyCost() { return collectEfficiencyCost; }
    public AtomAmo GetAtomCollectorWeightCost() { return collectWeightCost; }
    public AtomAmo GetParticleSpeedCost() { return particleSpeedCost; }
    public AtomAmo GetParticleStabilizationCost() { return particleStablizationCost; }

}