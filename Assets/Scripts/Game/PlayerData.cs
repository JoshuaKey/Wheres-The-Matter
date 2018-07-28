using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData  {
    // Player Data is responsible for holding and calculating data relevant to the player. Their current progression

    //[Serializable]
    //public class SerializableDictionary<T, U> {
    //    public Dictionary<T, U> dictionary;
    //}
    //[Serializable]
    //public class CraftableDictionary : SerializableDictionary<Craftable, int> { }

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
    public struct UpgradeData {
        public int level;
        public List<AtomAmo> cost;
        public float value;
    }

    [Serializable]
    public class UpgradeLevel {
        //[SerializeField] public int level;
        //[SerializeField] public List<AtomAmo> cost;
        //[SerializeField] public float value;
        public UpgradeData data;

        [Header("Info")]
        [SerializeField] private int maxLevel;
        [SerializeField] private string name;
        [SerializeField] [TextArea(1, 3)] private string desc;
        [SerializeField] private string measurementAbbr;
        
        [Header("Progression")]
        [SerializeField] private List<AnimationCurve> atomProgression;
        [SerializeField] private List<AnimationCurve> amoProgression;
        [SerializeField] private AnimationCurve valueProgression;

        public void Init() {
            data.level = 0;
            data.value = valueProgression.Evaluate(data.level);

            AtomAmo amo = new AtomAmo();
            data.cost = new List<AtomAmo>();
            for (int i = 0; i < atomProgression.Count; i++) {
                amo.amo = (int)amoProgression[i].Evaluate(data.level);

                int index = (int)atomProgression[i].Evaluate(data.level);
                if (index == 0) {
                    amo.atom = null;
                } else {
                    amo.atom = Game.Instance.gameData.FindAtom(index);
                }
                data.cost.Add(amo);
            }
        }
        public void Update() {
            data.value = valueProgression.Evaluate(data.level);

            data.cost.Clear();
            if (!IsMaxLevel()) {
                AtomAmo amo = new AtomAmo();
                for (int i = 0; i < atomProgression.Count; i++) {
                    amo.amo = (int)amoProgression[i].Evaluate(data.level);

                    int index = (int)atomProgression[i].Evaluate(data.level);
                    if (index == 0) {
                        amo.atom = null;
                    } else {
                        amo.atom = Game.Instance.gameData.FindAtom(index);
                    }

                    data.cost.Add(amo);
                }
            }
        }
        public bool Upgrade() {
            bool success = CanUpgrade();
            if (success) {
                for (int i = 0; i < data.cost.Count; i++) { // Use
                    AtomAmo atomCost = data.cost[i];
                    if (atomCost.atom == null) { continue; }

                    AtomData atomData = Game.Instance.gameData.FindAtomData(data.cost[i].atom.GetAtomicNumber());
                    atomData.Lose(data.cost[i].amo);
                }

                data.level++;
                data.value = valueProgression.Evaluate(data.level);

                data.cost.Clear();
                if (!IsMaxLevel()) {
                    AtomAmo amo = new AtomAmo();
                    for (int i = 0; i < atomProgression.Count; i++) {
                        amo.amo = (int)amoProgression[i].Evaluate(data.level);

                        int index = (int)atomProgression[i].Evaluate(data.level);
                        if (index == 0) {
                            amo.atom = null;
                        } else {
                            amo.atom = Game.Instance.gameData.FindAtom(index);
                        }

                        data.cost.Add(amo);
                    }
                }
            }

            return success;
        }
        public bool CanUpgrade() {
            if(IsMaxLevel()) { return false; }

            for (int i = 0; i < data.cost.Count; i++) { // Validation
                AtomAmo atomCost = data.cost[i];
                if (atomCost.atom == null) { continue; }

                AtomData atomData = Game.Instance.gameData.FindAtomData(atomCost.atom.GetAtomicNumber());
                if (atomData.GetCurrAmo() < atomCost.amo) {
                    return false;
                }
            }
            return true;
        }

        public bool IsMaxLevel() { return data.level == maxLevel;}
        public int GetMaxLevel() { return maxLevel; }
        public string GetName() { return name; }
        public string GetDesc() { return desc; }
        public string GetMeasurement() { return measurementAbbr; }

        public int GetLevel() { return data.level; }
        public float GetValue() { return data.value; }
        public List<AtomAmo> GetCost() { return data.cost; }

        public float GetNextValue() { return valueProgression.Evaluate(data.level + 1); }
        
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

        if (OnCollectRadiusChange != null) {
            OnCollectRadiusChange(radiusUpgrade.GetValue());
        }
        if (OnCollectSpeedChange != null) {
            OnCollectSpeedChange(speedUpgrade.GetValue());
        }
        if (OnCollectEfficiencyChange != null) {
            OnCollectEfficiencyChange(efficiencyUpgrade.GetValue());
        }
        if (OnCollectWeightChange != null) {
            OnCollectWeightChange(weightUpgrade.GetValue());
        }
        if (OnParticleSpeedChange != null) {
            OnParticleSpeedChange(accelerationUpgrade.GetValue());
        }
        if (OnParticleStabilityChange != null) {
            OnParticleStabilityChange(stabilityUpgrade.GetValue());
        }
        if (OnTimeDilationChange != null) {
            OnTimeDilationChange(timeUpgrade.GetValue());
        }
    }

    public void Load(float money, UpgradeData[] upgradeData, Dictionary<string, int> craftableAmo) {

        this.money = money;
        if (OnMoneyChange != null) {
            OnMoneyChange(money);
        }

        speedUpgrade.data = upgradeData[0];
        if (OnCollectSpeedChange != null) {
            OnCollectSpeedChange(upgradeData[0].value);
        }

        radiusUpgrade.data = upgradeData[1];
        if (OnCollectRadiusChange != null) {
            OnCollectRadiusChange(upgradeData[1].value);
        }

        efficiencyUpgrade.data = upgradeData[2];
        if (OnCollectEfficiencyChange != null) {
            OnCollectEfficiencyChange(upgradeData[2].value);
        }

        weightUpgrade.data = upgradeData[3];
        if (OnCollectWeightChange != null) {
            OnCollectWeightChange(upgradeData[3].value);
        }

        accelerationUpgrade.data = upgradeData[4];
        if (OnParticleSpeedChange != null) {
            OnParticleSpeedChange(upgradeData[4].value);
        }

        stabilityUpgrade.data = upgradeData[5];
        if (OnParticleStabilityChange != null) {
            OnParticleStabilityChange(upgradeData[5].value);
        }

        timeUpgrade.data = upgradeData[6];
        if (OnTimeDilationChange != null) {
            OnTimeDilationChange(upgradeData[6].value);
        }

        var craftEnum = craftableAmo.GetEnumerator();
        while (craftEnum.MoveNext()) {
            var craft = craftEnum.Current;

            Craftable c = Game.Instance.gameData.FindCraftable(craft.Key);
            craftables[c] = craft.Value;
        }
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
            //float halfLife = targetInfo.GetHalfLife() + timeUpgrade.GetValue();
            //float multiple = 1 / Mathf.Sqrt(1 - (stabilityUpgrade.GetValue() * stabilityUpgrade.GetValue()) / (100 * 100));
            //info.stability = AtomInfo.GetStability(halfLife * multiple * multiple);

            //\frac{\left(\left(t\cdot\left(a-v\right)+v\right)^{\sqrt{\frac{x}{30}}}\right)}{525600}
            float halflife = targetInfo.GetHalfLife() + timeUpgrade.GetValue();
            if(halflife < 0) {
                info.stability = 0f;
            } else {

                float yearPercentage = 5256000; // 10 years ish
                float t = .001f;

                float stableValue = stabilityUpgrade.GetValue();

                float lerp = Mathf.Lerp(halflife, yearPercentage, t);
                float power = Mathf.Sqrt(stableValue / 30);
                float stability = Mathf.Pow(lerp, power);
                info.stability = Mathf.Clamp01(stability / yearPercentage);
            }
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
            //float halfLife = targetInfo.GetHalfLife() + timeUpgrade.GetValue();
            //float multiple = 1 / Mathf.Sqrt(1 - (stabilityUpgrade.GetValue() * stabilityUpgrade.GetValue()) / (100 * 100));
            //info.stability = AtomInfo.GetStability(halfLife * multiple * multiple);

            float halflife = targetInfo.GetHalfLife() + timeUpgrade.GetValue();
            if (halflife < 0) {
                info.stability = 0f;
            } else {

                float yearPercentage = 5256000; // 10 years ish
                float t = .001f;

                float stableValue = stabilityUpgrade.GetValue();

                float lerp = Mathf.Lerp(halflife, yearPercentage, t);
                float power = Mathf.Sqrt(stableValue / 30);
                float stability = Mathf.Pow(lerp, power);
                info.stability = Mathf.Clamp01(stability / yearPercentage);
            }
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

    public void Story() {
        switch (Game.Instance.story.GetChapter()) {
            case 3:
                goto case 1;
            case 2:
                goto case 1;
            case 1:
                radiusUpgrade.data.level = 2;
                speedUpgrade.data.level = speedUpgrade.GetMaxLevel() / 2;
                efficiencyUpgrade.data.level = efficiencyUpgrade.GetMaxLevel() / 2;
                weightUpgrade.data.level = 0;
                accelerationUpgrade.data.level = 0;
                stabilityUpgrade.data.level = 0;
                timeUpgrade.data.level = 0;

                radiusUpgrade.Update();
                if (OnCollectRadiusChange != null) {
                    OnCollectRadiusChange(radiusUpgrade.GetValue());
                }
                speedUpgrade.Update();
                if (OnCollectSpeedChange != null) {
                    OnCollectSpeedChange(speedUpgrade.GetValue());
                }
                efficiencyUpgrade.Update();
                if (OnCollectEfficiencyChange != null) {
                    OnCollectEfficiencyChange(efficiencyUpgrade.GetValue());
                }
                weightUpgrade.Update();
                if (OnCollectWeightChange != null) {
                    OnCollectWeightChange(weightUpgrade.GetValue());
                }
                accelerationUpgrade.Update();
                if (OnParticleSpeedChange != null) {
                    OnParticleSpeedChange(accelerationUpgrade.GetValue());
                }
                stabilityUpgrade.Update();
                if (OnParticleStabilityChange != null) {
                    OnParticleStabilityChange(stabilityUpgrade.GetValue());
                }
                timeUpgrade.Update();
                if (OnTimeDilationChange != null) {
                    OnTimeDilationChange(timeUpgrade.GetValue());
                }
                break;
            default:
                Reset();
                break;
        }
    }
    public void Reset() {
        radiusUpgrade.data.level = 0;
        speedUpgrade.data.level = 0;
        efficiencyUpgrade.data.level = 0;
        weightUpgrade.data.level = 0;
        accelerationUpgrade.data.level = 0;
        stabilityUpgrade.data.level = 0;
        timeUpgrade.data.level = 0;

        radiusUpgrade.Update();
        if (OnCollectRadiusChange != null) {
            OnCollectRadiusChange(radiusUpgrade.GetValue());
        }
        speedUpgrade.Update();
        if (OnCollectSpeedChange != null) {
            OnCollectSpeedChange(speedUpgrade.GetValue());
        }
        efficiencyUpgrade.Update();
        if (OnCollectEfficiencyChange != null) {
            OnCollectEfficiencyChange(efficiencyUpgrade.GetValue());
        }
        weightUpgrade.Update();
        if (OnCollectWeightChange != null) {
            OnCollectWeightChange(weightUpgrade.GetValue());
        }
        accelerationUpgrade.Update();
        if (OnParticleSpeedChange != null) {
            OnParticleSpeedChange(accelerationUpgrade.GetValue());
        }
        stabilityUpgrade.Update();
        if (OnParticleStabilityChange != null) {
            OnParticleStabilityChange(stabilityUpgrade.GetValue());
        }
        timeUpgrade.Update();
        if (OnTimeDilationChange != null) {
            OnTimeDilationChange(timeUpgrade.GetValue());
        }
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
    public UpgradeLevel GetUpgrade(UpgradeType type) {
        UpgradeLevel upgrade = null;

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
        }

        return upgrade;
    }
    public OnFloatChange GetUpgradeDelegate(UpgradeType type) {
        OnFloatChange upgradeChange;

        switch (type) {
            case UpgradeType.Collect_Speed:
                upgradeChange = OnCollectSpeedChange;
                break;
            case UpgradeType.Collect_Radius:
                upgradeChange = OnCollectRadiusChange;
                break;
            case UpgradeType.Collect_Efficiency:
                upgradeChange = OnCollectEfficiencyChange;
                break;
            case UpgradeType.Collect_Weight:
                upgradeChange = OnCollectWeightChange;
                break;
            case UpgradeType.Particle_Speed:
                upgradeChange = OnParticleSpeedChange;
                break;
            case UpgradeType.Particle_Stability:
                upgradeChange = OnParticleStabilityChange;
                break;
            case UpgradeType.Time_Dilation:
                upgradeChange = OnTimeDilationChange;
                break;
            default:
                return null;
        }
        return upgradeChange;
    }

    public int GetCraftableAmount(Craftable c) {
        int amo = 0;
        if(craftables.TryGetValue(c, out amo)) {
            return amo;
        }
        return amo;
    }
    public float GetMoney() { return money; }

    public void Save(SaveData s) {
        s.money = money;

        //s.craftables = craftables;
        var enumerator = craftables.GetEnumerator();
        while (enumerator.MoveNext()) {
            var c = enumerator.Current;
            s.craftables.Add(c.Key);
            s.craftableAmo.Add(c.Value);
        }

        //s.craftableAmo = craftables;

        for(int i = 0; i < UpgradeTypeAmount; i++) {
            s.upgradeData.Add(GetUpgrade((UpgradeType) i).data);
        }
       
    }
    public void Load(SaveData s) {
        money = s.money;
        if(OnMoneyChange != null) {
            OnMoneyChange(money);
        }

        craftables = new Dictionary<Craftable, int>();
        for(int i = 0; i < s.craftableAmo.Count; i++) {
            Craftable c = s.craftables[i];
            int amo = s.craftableAmo[i];
            craftables[c] = amo;
        }

        //craftables = s.craftables ?? new SerializableDictionary<Craftable, int>();

        for (int i = 0; i < UpgradeTypeAmount; i++) {

            var data = s.upgradeData[i];
            var upgrade = GetUpgrade((UpgradeType)i);

            upgrade.data = data;

            var upgradeDelegate = GetUpgradeDelegate((UpgradeType)i);
            if(upgradeDelegate != null) {
                upgradeDelegate(upgrade.data.value);
            }
        }
    }
}