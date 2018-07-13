using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour {

    // Display all Craftable on left
    [Header("Craftable Info")]
    [SerializeField] Image craftableImage;
    [SerializeField] TextMeshProUGUI craftableNameText;
    [SerializeField] TextMeshProUGUI craftableInfoText;
    [SerializeField] TextMeshProUGUI atomsRequiredText;
    [SerializeField] TextMeshProUGUI atomAmountText;
    [SerializeField] TextMeshProUGUI currAtomAmountText;
    [SerializeField] RectTransform atomNeededRect;
    [SerializeField] Button craftBtn;
    [SerializeField] Button craftBtnx10;
    [SerializeField] Button craftBtnx100;
    [SerializeField] Button sellBtn;
    [SerializeField] Button sellBtnx10;
    [SerializeField] Button sellBtnx100;

    [Header("Other")]
    [SerializeField] ResultUI resultUI;
    [SerializeField] Toggle resultsToggle;
    [SerializeField] AudioClip choiceClickSound;

    [Header("Craftable List")]
    [SerializeField] RectTransform craftableListRect;
    [SerializeField] ChoiceOption choicePrefab;

    private Dictionary<Craftable, ChoiceOption> craftableChoices = new Dictionary<Craftable, ChoiceOption>();
    private List<Craftable> undiscoveredOptions = new List<Craftable>();

    private Craftable currCraftable;

    private float startPos = 0;
    private float yPos = 0;

    private void Start() {
        // Spawn all Craftables
        startPos = -5;
        yPos = startPos;
        for (int i = 0; i < Game.Instance.gameData.GetCraftableAmount(); i++) {
            Craftable c = Game.Instance.gameData.GetCraftable(i);

            OnCraftableDiscover(c, 0);
            //var craftablChoice = Instantiate(choicePrefab);

            //// Pos
            //craftablChoice.transform.SetParent(craftableListRect, false);
            //craftablChoice.transform.localScale = Vector3.one;

            //// Data / Functionality
            //SetCraftableChoice(craftablChoice, c);
            //craftablChoice.SetButtonEvent(() => { // Event should never change
            //    SetCraftable(c);
            //    AudioManager.Instance.PlaySound(choiceClickSound);
            //});
            //craftablChoice.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);

            //craftableChoices.Add(c, craftablChoice);

            //bool canCraft = true;
            //var atomsForCraft = c.GetAtomsForProduction();
            //for(int y = 0; y < atomsForCraft.Length; y++) {
            //    var atomAmo = atomsForCraft[y];
            //    if (!Game.Instance.gameData.FindAtomInfo(atomAmo.atom.GetAtomicNumber()).IsDiscovered()) {
            //        canCraft = false;
            //        break;
            //    }
            //}
            //if (canCraft) {
            //    AddCraftable(craftablChoice);
            //} else {
            //    undiscoveredOptions.Add(c);
            //    craftablChoice.gameObject.SetActive(false);
            //}
        }
        //// Height of ScrollView
        //var sizeDelta = craftableListRect.sizeDelta;
        //sizeDelta.y = startPos - yPos + 10;
        //craftableListRect.sizeDelta = sizeDelta;

        RemoveCraftable();

        Game.Instance.gameData.OnAtomDiscover += OnAtomDiscover;
        Game.Instance.gameData.OnCraftableDiscover += OnCraftableDiscover;
    }

    private void OnAtomDiscover(Atom a, float amo) {
        if(undiscoveredOptions.Count == 0) {
            Game.Instance.gameData.OnAtomDiscover -= OnAtomDiscover;
            return;
        }

        for (int i = 0; i < undiscoveredOptions.Count; i++) {
            Craftable c = undiscoveredOptions[i];

            bool canCraft = true;
            var atomsForCraft = c.GetAtomsForProduction();
            for (int y = 0; y < atomsForCraft.Length; y++) {
                var atomAmo = atomsForCraft[y];
                if (!Game.Instance.gameData.FindAtomInfo(atomAmo.atom.GetAtomicNumber()).IsDiscovered()) {
                    canCraft = false;
                    break;
                }
            }
            if (canCraft) {
                AddCraftable(craftableChoices[c]);
                undiscoveredOptions.RemoveAt(i);
                i--;
            } 
        }
    }
    private void OnCraftableDiscover(Craftable c, float amo) {
        var craftablChoice = Instantiate(choicePrefab);

        // Pos
        craftablChoice.transform.SetParent(craftableListRect, false);
        craftablChoice.transform.localScale = Vector3.one;

        // Data / Functionality
        SetCraftableChoice(craftablChoice, c);
        craftablChoice.SetButtonEvent(() => { // Event should never change
            SetCraftable(c);
            AudioManager.Instance.PlaySound(choiceClickSound);
        });
        craftablChoice.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);

        craftableChoices.Add(c, craftablChoice);

        bool canCraft = true;
        var atomsForCraft = c.GetAtomsForProduction();
        for (int y = 0; y < atomsForCraft.Length; y++) {
            var atomAmo = atomsForCraft[y];
            if (!Game.Instance.gameData.FindAtomInfo(atomAmo.atom.GetAtomicNumber()).IsDiscovered()) {
                canCraft = false;
                break;
            }
        }
        if (canCraft) {
            AddCraftable(craftablChoice);
        } else {
            undiscoveredOptions.Add(c);
            craftablChoice.gameObject.SetActive(false);
        }
    }

    private void AddCraftable(ChoiceOption c) {
        var pos = c.transform.localPosition;
        pos.y = yPos;
        c.transform.localPosition = pos;
        yPos -= 50;


        var sizeDelta = craftableListRect.sizeDelta;
        sizeDelta.y = startPos - yPos + 10;
        craftableListRect.sizeDelta = sizeDelta;

        c.gameObject.SetActive(true);
    }

    public void SetCraftable(Craftable c) {
        RemoveCraftable();

        ChoiceOption choiceOption;
        if (craftableChoices.TryGetValue(c, out choiceOption)) {
            choiceOption.SetButtonEvent(() => {
                RemoveCraftable();
                AudioManager.Instance.PlaySound(choiceClickSound);
            });
            choiceOption.SetColors(ChoiceOption.defaultPressedColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultNormalColor);
            choiceOption.SetFocus(false);
        }

        currCraftable = c;

        int amo = Game.Instance.playerData.GetCraftableAmount(c);

        craftableImage.sprite = c.GetSprite();

        craftableNameText.text = c.GetName();
        craftableInfoText.text = "Price: $" + c.GetPrice() + "\nCurr Amo: " + amo;

        StringBuilder atomName = new StringBuilder();
        StringBuilder atomAmo = new StringBuilder();
        StringBuilder atomHave = new StringBuilder();
        var atoms = c.GetAtomsForProduction();
        for (int i = 0; i < atoms.Length; i++) {
            Atom a = atoms[i].atom;

            AtomData data = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
            if (data.GetCurrAmo() >= atoms[i].amo) {
                atomName.Append( a.GetName() + "\n");
                atomAmo.Append(  atoms[i].amo + "\n");
                atomHave.Append( data.GetCurrAmo() + "\n");
            } else {
                atomName.Append("<color=#ff8080>" + a.GetName() + "\n</color>");
                atomAmo.Append( "<color=#ff8080>" + atoms[i].amo + "\n</color>");
                atomHave.Append("<color=#ff8080>" + data.GetCurrAmo() + "\n</color>");
            }
        }

        atomsRequiredText.text = atomName.ToString();
        atomAmountText.text = atomAmo.ToString();
        currAtomAmountText.text = atomHave.ToString();

        var size = atomNeededRect.sizeDelta;
        size.y = atoms.Length * 36;
        atomNeededRect.sizeDelta = size;

        bool canCraft = Game.Instance.playerData.CanCraft(c);

        craftBtn.interactable = canCraft;
        craftBtnx10.interactable = canCraft;
        craftBtnx100.interactable = canCraft;

        bool canSell = Game.Instance.playerData.GetCraftableAmount(c) > 0;

        sellBtn.interactable = canSell;
        sellBtnx10.interactable = canSell;
        sellBtnx100.interactable = canSell;
    }
    public void RemoveCraftable() {
        ChoiceOption choiceOption;
        if (currCraftable != null && craftableChoices.TryGetValue(currCraftable, out choiceOption)) {
            var craftable = currCraftable;
            choiceOption.SetButtonEvent(() => {
                SetCraftable(craftable);
                AudioManager.Instance.PlaySound(choiceClickSound);
            });
            choiceOption.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);
            choiceOption.SetFocus(false);
        } 

        craftableImage.sprite = Game.Instance.gameData.GetUknownInfo().GetImage();

        craftableNameText.text = "???";
        craftableInfoText.text = "Price: \nCurr Amo: ";
        atomsRequiredText.text = "";
        atomAmountText.text = "";
        currAtomAmountText.text = "";

        craftBtn.interactable = false;
        craftBtnx10.interactable = false;
        craftBtnx100.interactable = false;

        sellBtn.interactable = false;
        sellBtnx10.interactable = false;
        sellBtnx100.interactable = false;

        currCraftable = null;
    }
    public void SetCraftableChoice(ChoiceOption choice, Craftable c) {
        int amo = Game.Instance.playerData.GetCraftableAmount(c);
        choice.SetText(c.GetName() + " (" + amo  + ")\n<size=80%>    Price: $" + c.GetPrice());
    }

    public void Craft(int amount) {
        var result = Game.Instance.playerData.Craft(currCraftable, amount);

        resultUI.Setup(null, result.atomsUsed);

        var temp = currCraftable;
        Refresh();
        SetCraftable(temp);
    }
    public void Sell(int amount) {
        Game.Instance.playerData.Sell(currCraftable, amount);
        //int amo = 
        //resultUI.Setup(null, result.atomsUsed);

        var temp = currCraftable;
        Refresh();
        SetCraftable(temp);
    }

    public void Refresh() {
        RemoveCraftable();
        
        for (int i = 0; i < craftableChoices.Count; i++) {
            Craftable c = Game.Instance.gameData.GetCraftable(i);

            SetCraftableChoice(craftableChoices.Values.ElementAt(i), c);
        }
    }

    private void OnEnable() {
        Refresh();
    }

}
