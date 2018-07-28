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

    private PlayerData.UpgradeType currUpgradeType;
    private ChoiceOption speedChoice;
    private ChoiceOption radiusChoice;
    private ChoiceOption efficiencyChoice;
    private ChoiceOption weightChoice;
    private ChoiceOption accelerationChoice;
    private ChoiceOption stabilityChoice;
    private ChoiceOption timeDilationChoice;

    private void Start() {
        float startPos = -5;
        float yPos = startPos;

        for(int i = 0; i < PlayerData.UpgradeTypeAmount; i++) {
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
            SetChoiceDisplay(choice, (PlayerData.UpgradeType)i);
            int temp = i;
            choice.SetButtonEvent(() => {
                SetUpgradeInfo((PlayerData.UpgradeType)temp);
                AudioManager.Instance.PlayUISound(choiceClick);
            });
            choice.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);
            choice.SetTextAlignment(TextAlignmentOptions.Center);

            choices.Add(choice);

            switch (i) {
                case 0:
                    speedChoice = choice;
                    speedChoice.SetInteractable(Game.Instance.story.GetChapter() > 4);
                    break;
                case 1:
                    radiusChoice = choice;
                    radiusChoice.SetInteractable(Game.Instance.story.GetChapter() > 4);
                    break;
                case 2:
                    efficiencyChoice = choice;
                    efficiencyChoice.SetInteractable(Game.Instance.story.GetChapter() > 4);
                    break;
                case 3:
                    //weightChoice = choice;
                    break;
                case 4:
                    accelerationChoice = choice;
                    accelerationChoice.gameObject.SetActive(Game.Instance.story.GetChapter() > 4);
                    break;
                case 5:
                    stabilityChoice = choice;
                    stabilityChoice.gameObject.SetActive(Game.Instance.story.GetChapter() > 4);
                    break;
                case 6:
                    timeDilationChoice = choice;

                    if (!Game.Instance.gameData.FindAtomData(119).IsDiscovered() &&
                        !Game.Instance.gameData.FindAtomData(120).IsDiscovered() &&
                        !Game.Instance.gameData.FindAtomData(121).IsDiscovered()) {
                        timeDilationChoice.gameObject.SetActive(false);
                        Game.Instance.gameData.OnAtomDiscover += CheckUpgradeUnlock;
                    }
                    break;
            }
        }

        var sizeD = upgradeListContent.sizeDelta;
        sizeD.y = startPos - yPos + 10;
        upgradeListContent.sizeDelta = sizeD;
    }

    public void Story() {
        switch (Game.Instance.story.GetChapter()) {
            default:
                if(speedChoice != null) {
                    speedChoice.SetInteractable(true);
                    radiusChoice.SetInteractable(true);
                    efficiencyChoice.SetInteractable(true);
                    accelerationChoice.gameObject.SetActive(true);
                    stabilityChoice.gameObject.SetActive(true);
                }               
                break;
        }
    }

    public void CheckUpgradeUnlock(Atom a, float amo) {
        if(a.GetAtomicNumber() > 118) {
            timeDilationChoice.gameObject.SetActive(true);

            Game.Instance.gameData.OnAtomDiscover -= CheckUpgradeUnlock;
        } 
    }

    public void SetChoiceDisplay(ChoiceOption choice, PlayerData.UpgradeType type) {
        string text = "";
        switch (type) {
            case PlayerData.UpgradeType.Collect_Speed:
                text = "Collect Speed Lv. ";
                break;
            case PlayerData.UpgradeType.Collect_Radius:
                text = "Collect Radius Lv. ";
                break;
            case PlayerData.UpgradeType.Collect_Efficiency:
                text = "Collect Efficiency Lv. ";
                break;
            case PlayerData.UpgradeType.Collect_Weight:
                text = "Collect Weight Lv. ";
                break;
            case PlayerData.UpgradeType.Particle_Speed:
                text = "Particle Speed Lv. ";
                break;
            case PlayerData.UpgradeType.Particle_Stability:
                text = "Particle Stability Lv. ";
                break;
            case PlayerData.UpgradeType.Time_Dilation:
                text = "Time Dilation Lv. ";
                break;
        }

        if (Game.Instance.playerData.IsMaxLevel(type)) {
            choice.SetText(text + "MAX");
        } else {
            choice.SetText(text + Game.Instance.playerData.GetUpgradeLevel(type));
        }

    }

    public void SetUpgradeInfo(PlayerData.UpgradeType type) {
        RemoveUpgrade();
        if((int)type >= choices.Count) { return; }

        // Button Stuff
        ChoiceOption choiceOption = choices[(int)type];
        choiceOption.SetButtonEvent(() => {
            RemoveUpgrade();
            AudioManager.Instance.PlaySound(choiceClick);
        });
        choiceOption.SetColors(ChoiceOption.defaultPressedColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultNormalColor);
        choiceOption.SetFocus(false);
        currUpgradeType = type;

        // Display Stuff
        List<AtomAmo> atomsNeeded = Game.Instance.playerData.GetCost(type);

        upgradeName.text = Game.Instance.playerData.GetName(type);
        upgradeDesc.text = Game.Instance.playerData.GetDescription(type);
        upgradeCurrValue.text = "Curr Value: " + System.Math.Round(Game.Instance.playerData.GetValue(type), 3)
            + " " + Game.Instance.playerData.GetMeasurementAbbr(type);//currValue;
        upgradeNextValue.text = "Next Value: " + System.Math.Round(Game.Instance.playerData.GetNextValue(type), 3)
            + " " + Game.Instance.playerData.GetMeasurementAbbr(type);//nextValue;

        bool canCraft = true;
        for(int i = 0; i < atomsNeeded.Count; i++) {
            AtomAmo cost = atomsNeeded[i];
            if(cost.atom == null) { continue; }
            
            int amoHave = Game.Instance.gameData.FindAtomData(cost.atom.GetAtomicNumber()).GetCurrAmo();
            if (amoHave >= cost.amo) {
                upgradeAtomName.text = cost.atom.GetName() + "\n";
                upgradeAtomNeed.text = cost.amo + "\n";
                upgradeAtomHave.text = amoHave + "\n";
            } else {
                upgradeAtomName.text = "<color=#ff8080>" + cost.atom.GetName() + "\n</color>";
                upgradeAtomNeed.text = "<color=#ff8080>" + cost.amo + "\n</color>";
                upgradeAtomHave.text = "<color=#ff8080>" + amoHave + "\n</color>";
                canCraft = false;
            }
        }
        upgradeBtn.interactable = canCraft;
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
        List<AtomAmo> oldUsedAtoms = new List<AtomAmo>();
        List<AtomAmo> temp = Game.Instance.playerData.GetCost(currUpgradeType);
        for(int i = 0; i < temp.Count; i++) {
            oldUsedAtoms.Add(temp[i]);
        }

        bool success = Game.Instance.playerData.Upgrade(currUpgradeType);

        if (success) {
            resultUI.Setup(null, oldUsedAtoms);
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
        for(int i = 0; i < PlayerData.UpgradeTypeAmount && i < choices.Count; i++) {
            SetChoiceDisplay(choices[i], (PlayerData.UpgradeType)i);
        }
    }

}
