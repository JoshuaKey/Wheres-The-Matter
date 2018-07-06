using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour {

    [Header("Info")]
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] TextMeshProUGUI upgradeDesc;
    [SerializeField] TextMeshProUGUI upgradeCurrValue;
    [SerializeField] TextMeshProUGUI upgradeNextValue;

    [SerializeField] TextMeshProUGUI upgradeAtomName;
    [SerializeField] TextMeshProUGUI upgradeAtomNeed;
    [SerializeField] TextMeshProUGUI upgradeAtomHave;

    [Header("Other")]
    [SerializeField] Button upgradeBtn;
    [SerializeField] RectTransform upgradeListContent;
    [SerializeField] ChoiceOption choicePrefab;
    [SerializeField] ResultUI resultUI;
    [SerializeField] AudioClip choiceClick;

    private List<ChoiceOption> choices = new List<ChoiceOption>();

    public enum UpgradeType {
        Collect_Speed,
        Collect_Radius,
        Collect_Efficiency,
        Collect_Weight,
        Particle_Speed,
        Particle_Stability,
    }
    public static readonly int UpgradeTypeAmount = 6;

    private UpgradeType currUpgradeType;

    private void Start() {
        float startPos = -5;
        float yPos = startPos;

        for(int i = 0; i < UpgradeTypeAmount; i++) {
            // Instantiate
            var choice = Instantiate(choicePrefab);

            // Pos
            choice.transform.SetParent(upgradeListContent, false);
            choice.transform.localScale = Vector3.one;

            var pos = choice.transform.localPosition;
            pos.y = yPos;
            choice.transform.localPosition = pos;
            yPos -= 50;

            //Data
            SetChoiceDisplay(choice, (UpgradeType)i);
            int temp = i;
            choice.SetButtonEvent(() => {
                SetUpgradeInfo((UpgradeType)temp);
                AudioManager.Instance.PlayUISound(choiceClick);
            });
            choice.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);
            choice.SetTextAlignment(TextAlignmentOptions.Center);

            choices.Add(choice);
        }

        var sizeD = upgradeListContent.sizeDelta;
        sizeD.y = startPos - yPos + 10;
        upgradeListContent.sizeDelta = sizeD;
    }

    public void SetChoiceDisplay(ChoiceOption choice, UpgradeType type) {
        switch (type) {
            case UpgradeType.Collect_Speed:
                choice.SetText("Collect Speed Lv. " + 1);
                break;
            case UpgradeType.Collect_Radius:
                choice.SetText("Collect Radius Lv. " + 1);
                break;
            case UpgradeType.Collect_Efficiency:
                choice.SetText("Collect Efficiency Lv. " + 1);
                break;
            case UpgradeType.Collect_Weight:
                choice.SetText("Collect Weight Lv. " + 1);
                break;
            case UpgradeType.Particle_Speed:
                choice.SetText("Particle Speed Lv. " + 1);
                break;
            case UpgradeType.Particle_Stability:
                choice.SetText("Particle Stability Lv. " + 1);
                break;
        }
    }

    public void SetUpgradeInfo(UpgradeType type) {
        RemoveUpgrade();

        if((int)type >= choices.Count) { return; }

        ChoiceOption choiceOption = choices[(int)type];
        choiceOption.SetButtonEvent(() => {
            RemoveUpgrade();
            AudioManager.Instance.PlaySound(choiceClick);
        });
        choiceOption.SetColors(ChoiceOption.defaultPressedColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultNormalColor);
        choiceOption.SetFocus(false);
        currUpgradeType = type;

        string name = "", description = "";
        string currValue = "", nextValue ="";
        AtomAmo atomsNeeded = new AtomAmo();

        switch (type) {
            case UpgradeType.Collect_Speed:
                name = "Collect Speed";
                description = "How quickly the Atom Collector can collect atoms.\nSome objects take longer to collect than others.";
                currValue = Game.Instance.playerData.GetAtomCollectorSpeed()*100 +"%"; // Measurable?
                nextValue = Game.Instance.playerData.GetNextAtomCollectorSpeed()*100 + "%";
                atomsNeeded = Game.Instance.playerData.GetAtomCollectorSpeedCost();
                break;
            case UpgradeType.Collect_Radius:
                name = "Collect Radius";
                description = "How far the Atom Collector can collect. Measured in Meters.";
                currValue = Game.Instance.playerData.GetAtomCollectorRadius() + " m";
                nextValue = Game.Instance.playerData.GetNextAtomCollectorRadius() + " m";
                atomsNeeded = Game.Instance.playerData.GetAtomCollectorRadiusCost();
                break;
            case UpgradeType.Collect_Efficiency:
                name = "Collect Efficiency";
                description = "How many atoms the Atom Collector can collect at a time.";
                currValue = Game.Instance.playerData.GetAtomCollectorEfficiency() *100 + "%";
                nextValue = Game.Instance.playerData.GetNextAtomCollectorEfficiency() *100 + "%";
                atomsNeeded = Game.Instance.playerData.GetAtomCollectorEfficiencyCost();
                break;
            case UpgradeType.Collect_Weight:
                name = "Collect Weight";
                description = "Determines if the Atom Collector can collect a heavy atom.\nAn atom can be collected if it's weight (Protons + Neutrons) is less than the Collect Weight.";
                currValue += Game.Instance.playerData.GetAtomCollectorWeight();
                nextValue += Game.Instance.playerData.GetNextAtomCollectorWeight();
                atomsNeeded = Game.Instance.playerData.GetAtomCollectorWeightCost();
                break;
            case UpgradeType.Particle_Speed:
                name = "Particle Accelerator Speed";
                description = "How fast the Particle Accelerator runs.\nThe faster it is, the more likely splitting and combining will succeed.\nThis is measured as a percent of the Speed of Light.";
                currValue = Game.Instance.playerData.GetParticleSpeed() + "%";
                nextValue = Game.Instance.playerData.GetNextParticleSpeed() + "%";
                atomsNeeded = Game.Instance.playerData.GetParticleSpeedCost();
                break;
            case UpgradeType.Particle_Stability:
                name = "Particle Accelerator Stability";
                description = "How stable the Particle Accelerator is.\nThe higher it is, the more likely radioactive atoms will become stable.\nThis is measured as a percent of the Speed of Light.";
                currValue = Game.Instance.playerData.GetParticleStabilization() + "%";
                nextValue = Game.Instance.playerData.GetNextParticleStabilization() + "%";
                atomsNeeded = Game.Instance.playerData.GetParticleStabilizationCost();
                break;
        }

        upgradeName.text = name;
        upgradeDesc.text = description;
        upgradeCurrValue.text = "Curr Value: " + currValue;
        upgradeNextValue.text = "Next Value: " + nextValue;

        upgradeAtomName.text = atomsNeeded.atom.GetName();
        upgradeAtomNeed.text = "" + atomsNeeded.amo;
        int amoHave = Game.Instance.gameData.FindAtomData(atomsNeeded.atom.GetAtomicNumber()).GetCurrAmo();
        upgradeAtomHave.text = "" + amoHave;

        upgradeBtn.interactable = amoHave >= atomsNeeded.amo;
    }

    public void RemoveUpgrade() {
        if (choices.Count > (int)currUpgradeType) {
            ChoiceOption choiceOption = choices[(int)currUpgradeType];
            var type = currUpgradeType;
            choiceOption.SetButtonEvent(() => {
                SetUpgradeInfo(type);
                AudioManager.Instance.PlaySound(choiceClick);
            });
            choiceOption.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);
            choiceOption.SetFocus(false);
        }
        //if (choices.Count > (int)currUpgradeType) {
        //    choices[(int)currUpgradeType].SetInteractable(true);
        //}

        upgradeName.text = "Upgrade";
        upgradeDesc.text = "";
        upgradeCurrValue.text = "Curr Value: ";
        upgradeNextValue.text = "Next Value: ";

        upgradeAtomNeed.text = "";
        upgradeAtomName.text = "";
        upgradeAtomHave.text = "";

        upgradeBtn.interactable = false;
    }

    public void Upgrade() {
        AtomAmo used = new AtomAmo();
        bool success = false;

        switch (currUpgradeType) {
            case UpgradeType.Collect_Speed:
                used = Game.Instance.playerData.GetAtomCollectorSpeedCost();
                success = Game.Instance.playerData.UpgradeAtomCollectorSpeed();
                break;
            case UpgradeType.Collect_Radius:
                used = Game.Instance.playerData.GetAtomCollectorRadiusCost();
                success = Game.Instance.playerData.UpgradeAtomCollectorRadius();
                break;
            case UpgradeType.Collect_Efficiency:
                used = Game.Instance.playerData.GetAtomCollectorEfficiencyCost();
                success = Game.Instance.playerData.UpgradeAtomCollectorEfficiency();
                break;
            case UpgradeType.Collect_Weight:
                used = Game.Instance.playerData.GetAtomCollectorWeightCost();
                success = Game.Instance.playerData.UpgradeAtomCollectorWeight();
                break;
            case UpgradeType.Particle_Speed:
                used = Game.Instance.playerData.GetParticleSpeedCost();
                success = Game.Instance.playerData.UpgradeParticleSpeed();
                break;
            case UpgradeType.Particle_Stability:
                used = Game.Instance.playerData.GetParticleStabilizationCost();
                success = Game.Instance.playerData.UpgradeParticleStabilization();
                break;
        }

        if (success) {
            resultUI.Setup(null, new List<AtomAmo>() { used });
        }

        var type = currUpgradeType;
        Reset();
        SetUpgradeInfo(type);
    }

    private void OnEnable() {
        var type = currUpgradeType;
        Reset();
        if (choices.Count > 0) {
            SetUpgradeInfo(type);
        }
    }

    public void Reset() {
        RemoveUpgrade();
        for(int i = 0; i < UpgradeTypeAmount && i < choices.Count; i++) {
            SetChoiceDisplay(choices[i], (UpgradeType)i);
        }
    }

}
