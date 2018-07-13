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
    public struct AtomObject {
        public Atom atom;
        public AtomInfo info;
        public AtomData data;
    }

    [Serializable]
    public class UpgradeLevel {
        [SerializeField] public int level;
        [SerializeField] public List<AtomAmo> cost;
        [SerializeField] public float value;

        [Header("Info")]
        [SerializeField] private int maxLevel;
        [SerializeField] private string name;
        [SerializeField] [TextArea(1, 3)] private string desc;
        [SerializeField] private string measurementAbbr;

        [Header("Progression")]
        public List<AnimationCurve> atomProgression;
        public List<AnimationCurve> amoProgression;
        public AnimationCurve valueProgression;

        public void Init() {
            level = 0;
            value = valueProgression.Evaluate(level);

            AtomAmo amo = new AtomAmo();
            cost = new List<AtomAmo>();
            for (int i = 0; i < atomProgression.Count; i++) {
                amo.amo = (int)amoProgression[i].Evaluate(level);

                int index = (int)atomProgression[i].Evaluate(level);
                if (index == 0) {
                    amo.atom = null;
                } else {
                    amo.atom = Game.Instance.gameData.FindAtom(index);
                }
                cost.Add(amo);
            }
        }
        public bool Upgrade() {
            bool success = CanUpgrade();
            if (success) {
                for (int i = 0; i < cost.Count; i++) { // Use
                    AtomAmo atomCost = cost[i];
                    if (atomCost.atom == null) { continue; }

                    AtomData data = Game.Instance.gameData.FindAtomData(cost[i].atom.GetAtomicNumber());
                    data.Lose(cost[i].amo);
                }

                level++;
                value = valueProgression.Evaluate(level);

                cost.Clear();
                if (!IsMaxLevel()) {
                    AtomAmo amo = new AtomAmo();
                    for (int i = 0; i < atomProgression.Count; i++) {
                        amo.amo = (int)amoProgression[i].Evaluate(level);

                        int index = (int)atomProgression[i].Evaluate(level);
                        if (index == 0) {
                            amo.atom = null;
                        } else {
                            amo.atom = Game.Instance.gameData.FindAtom(index);
                        }

                        cost.Add(amo);
                    }
                }
            }

            return success;
        }
        public bool CanUpgrade() {
            if(IsMaxLevel()) { return false; }

            for (int i = 0; i < cost.Count; i++) { // Validation
                AtomAmo atomCost = cost[i];
                if (atomCost.atom == null) { continue; }

                AtomData data = Game.Instance.gameData.FindAtomData(atomCost.atom.GetAtomicNumber());
                if (data.GetCurrAmo() < atomCost.amo) {
                    return false;
                }
            }
            return true;
        }

        public bool IsMaxLevel() { return level == maxLevel;}
        public string GetName() { return name; }
        public string GetDesc() { return desc; }
        public string GetMeasurement() { return measurementAbbr; }

        public int GetLevel() { return level; }
        public float GetValue() { return value; }
        public List<AtomAmo> GetCost() { return cost; }

        public float GetNextValue() { return valueProgression.Evaluate(level + 1); }

    } 

    public enum UpgradeType {
        Collect_Speed = 0,
        Collect_Radius = 1,
        Collect_Efficiency = 2,
        Collect_Weight = 3,
        Particle_Speed = 4,
        Particle_Stability = 5,
        Time_Dilation = 6,
    }
    public static readonly int UpgradeTypeAmount = 7;

    [SerializeField] private UpgradeLevel radiusUpgrade;
    [SerializeField] private UpgradeLevel speedUpgrade;
    [SerializeField] private UpgradeLevel efficiencyUpgrade;
    [SerializeField] private UpgradeLevel weightUpgrade;
    [SerializeField] private UpgradeLevel accelerationUpgrade;
    [SerializeField] private UpgradeLevel stabilityUpgrade;
    [SerializeField] private UpgradeLevel timeUpgrade;

    private float money;

    private Dictionary<Craftable, int> craftables = new Dictionary<Craftable, int>();

    public delegate void OnFloatChange(float value);
    public delegate void OnAtomChange(Atom atom, float value);
    public delegate void OnCraftableChange(Craftable atom, float value);

    public event OnFloatChange OnMoneyChange;
    public event OnFloatChange OnCollectRadiusChange;
    public event OnFloatChange OnCollectSpeedChange;
    public event OnFloatChange OnCollectEfficiencyChange;
    public event OnFloatChange OnCollectWeightChange;
    public event OnFloatChange OnParticleSpeedChange;
    public event OnFloatChange OnParticleStabilityChange;
    public event OnFloatChange OnTimeDilationChange;

    public event OnAtomChange OnAtomSplit;
    public event OnAtomChange OnAtomCombine;
    public event OnAtomChange OnAtomRename;

    public event OnCraftableChange OnCraftableProduced;
    public event OnCraftableChange OnCraftableSold;

    public void Init() {
        radiusUpgrade.Init();
        speedUpgrade.Init();
        efficiencyUpgrade.Init();
        weightUpgrade.Init();
        accelerationUpgrade.Init();
        stabilityUpgrade.Init();
        timeUpgrade.Init();
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
        } else {
            info.targetAtom = null;
            return info;
        }
        AtomInfo targetInfo = Game.Instance.gameData.FindAtomInfo(maxProtons);

        int minProtons = Mathf.Min(aAmo, bAmo); // Min of Atoms
        int minNeutrons = (totalANeutrons + totalBNeutrons) / targetInfo.GetNeutrons();
        info.amo = Mathf.Min(minProtons, minNeutrons);

        // 119 / 10 -> 11.9% Speed
        // 1 - (11.9 / 12)  -> .008333 About 1 / 12
        // 
        // At 10% speed of light, Titanium -> Berkelium produces an estimated 1 / 1 billion atoms of Element 119
        float estimatedSpeed = maxProtons / 10f;
        info.success = Mathf.Clamp01(1 - estimatedSpeed / accelerationUpgrade.GetValue());

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
            float halfLife = targetInfo.GetHalfLife() + timeUpgrade.GetValue();
            float multiple = 1 / Mathf.Sqrt(1 - (stabilityUpgrade.GetValue() * stabilityUpgrade.GetValue()) / (100 * 100));
            info.stability = AtomInfo.GetStability(halfLife * multiple * multiple);
        }

        return info;
    }
    public AtomCollisionResult ProduceCombine(Atom a, Atom b, int aAmo, int bAmo) {
        AtomCollision info = EstimateCombine(a, b, aAmo, bAmo);

        AtomCollisionResult result = new AtomCollisionResult();
        result.atomsProduced = new List<AtomAmo>();
        result.atomsUsed = new List<AtomAmo>();

        AtomData aData = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
        AtomData bData = Game.Instance.gameData.FindAtomData(b.GetAtomicNumber());
        int usedAmo = Mathf.Min(Mathf.Min(aAmo, aData.GetCurrAmo()), Mathf.Min(bAmo, bData.GetCurrAmo()));

        Atom target = info.targetAtom;
        int maxAmo = info.amo;

        float successChance = info.success;
        float stabilityChance = 1.0f;

        stabilityChance = info.stability;

        int produced = (int)(maxAmo * successChance);
        int stabilized = (int)(produced * stabilityChance);

        AtomAmo atomAmo = new AtomAmo();
        atomAmo.amo = stabilized;
        atomAmo.atom = target;
        result.atomsProduced.Add(atomAmo);

        Game.Instance.Absorb(target, stabilized); 

        AtomAmo atomAUsed = new AtomAmo();
        atomAUsed.atom = a;
        atomAUsed.amo = usedAmo;
        result.atomsUsed.Add(atomAUsed);

        AtomAmo atomBUsed = new AtomAmo();
        atomBUsed.atom = b;
        atomBUsed.amo = usedAmo;
        result.atomsUsed.Add(atomBUsed);

        Game.Instance.Use(a, usedAmo);
        Game.Instance.Use(b, usedAmo);

        if(OnAtomCombine != null){
            OnAtomCombine(target, stabilized);
        }

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
        info.success = Mathf.Clamp01(1 - estimatedSpeed / accelerationUpgrade.GetValue());

         // Stability
        if (targetInfo.GetHalfLife() == 0f) {
            info.stability = 1.0f;
        } else {
            float halfLife = targetInfo.GetHalfLife() + timeUpgrade.GetValue();
            float multiple = 1 / Mathf.Sqrt(1 - (stabilityUpgrade.GetValue() * stabilityUpgrade.GetValue()) / (100 * 100));
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


        if (OnAtomSplit != null) {
            OnAtomSplit(target, stabilized);
        }

        return result;
    }

    public void RenameAtom(int atomicNumber, string name, string abbreviation) {
        Atom atom = Game.Instance.gameData.FindAtom(atomicNumber);

        if (atom != null && atom.CanBeRenamed()) { // Already Exists, Renames
            atom.Rename(name, abbreviation);

            if (OnAtomRename != null) {
                OnAtomRename(atom, 0);
            }

            var craftable = Craftable.CreateNewBlock(atom);
            Game.Instance.gameData.AddCraftable(craftable);
        }
    }

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

        if (OnCraftableProduced != null) {
            OnCraftableProduced(c, amount);
        }

        return result;
    }

    public int Sell(Craftable c, int amo) {
        int amoOfCraftables;
        if (craftables.TryGetValue(c, out amoOfCraftables)) {
            if (amo > amoOfCraftables) {
                amo = amoOfCraftables;
            } 

            craftables[c] = amoOfCraftables - amo;
            money += c.GetPrice() * amo;

            if (OnMoneyChange != null) {
                OnMoneyChange(money);
            }


            if (OnCraftableSold != null) {
                OnCraftableSold(c, amo);
            }

            return amo;
        }
        return 0;
    }

    public bool Upgrade(UpgradeType type) {
        UpgradeLevel upgrade;
        OnFloatChange upgradeChange;   

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                upgradeChange = OnCollectSpeedChange;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                upgradeChange = OnCollectRadiusChange;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                upgradeChange = OnCollectEfficiencyChange;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                upgradeChange = OnCollectWeightChange;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                upgradeChange = OnParticleSpeedChange;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                upgradeChange = OnParticleStabilityChange;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                upgradeChange = OnTimeDilationChange;
                break;
            default:
                return false;
        }

        if (upgrade.Upgrade()) {
            if(upgradeChange != null) {
                upgradeChange(upgrade.GetValue());
            }
            return true;
        }
        return false;
    }
    public bool CanUpgrade(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return false;
        }

        return upgrade.CanUpgrade();
    }
    public int GetUpgradeLevel(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return 0;
        }

        return upgrade.GetLevel();
    }
    public float GetValue(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return 0f;
        }

        return upgrade.GetValue();
    }
    public float GetNextValue(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return 0f;
        }

        return upgrade.GetNextValue();
    }
    public List<AtomAmo> GetCost(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return null;
        }

        return upgrade.GetCost();
    }
    public bool IsMaxLevel(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return false;
        }

        return upgrade.IsMaxLevel();
    }
    public string GetName(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return "";
        }

        return upgrade.GetName();
    }
    public string GetDescription(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return "";
        }

        return upgrade.GetDesc();
    }
    public string GetMeasurementAbbr(UpgradeType type) {
        UpgradeLevel upgrade;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgrade = speedUpgrade;
                break;
            case UpgradeType.Collect_Radius:
                upgrade = radiusUpgrade;
                break;
            case UpgradeType.Collect_Efficiency:
                upgrade = efficiencyUpgrade;
                break;
            case UpgradeType.Collect_Weight:
                upgrade = weightUpgrade;
                break;
            case UpgradeType.Particle_Speed:
                upgrade = accelerationUpgrade;
                break;
            case UpgradeType.Particle_Stability:
                upgrade = stabilityUpgrade;
                break;
            case UpgradeType.Time_Dilation:
                upgrade = timeUpgrade;
                break;
            default:
                return "";
        }

        return upgrade.GetMeasurement();
    }

    public int GetCraftableAmount(Craftable c) {
        int amo = 0;
        if(craftables.TryGetValue(c, out amo)) {
            return amo;
        }
        return amo;
    }
    public float GetMoney() { return money; }
}