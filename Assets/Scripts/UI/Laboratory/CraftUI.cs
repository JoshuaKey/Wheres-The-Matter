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
    [SerializeField] Button craftBtn;
    [SerializeField] Button craftBtnx10;
    [SerializeField] Button craftBtnx100;

    [Header("Other")]
    [SerializeField] ResultUI resultUI;
    [SerializeField] Toggle resultsToggle;
    [SerializeField] AudioClip choiceClickSound;

    [Header("Craftable List")]
    [SerializeField] RectTransform craftableListRect;
    [SerializeField] ChoiceOption craftableChoicePrefab;

    private List<ChoiceOption> craftableChoices = new List<ChoiceOption>();

    private Craftable currCraftable;

    private void Start() {
        // Spawn all Craftables
        float yPos = craftableChoicePrefab.transform.position.y - 5;
        for (int i = 0; i < Game.Instance.gameData.GetCraftableAmount(); i++) {
            Craftable c = Game.Instance.gameData.GetCraftable(i);

            var craftablChoice = Instantiate(craftableChoicePrefab);

            // Pos
            craftablChoice.transform.SetParent(craftableListRect, false);
            craftablChoice.transform.localScale = Vector3.one;

            var pos = craftablChoice.transform.localPosition;
            pos.y = yPos;
            craftablChoice.transform.localPosition = pos;
            yPos -= 50;

            // Data / Functionality
            SetCraftableChoice(craftablChoice, c);
            craftablChoice.SetButtonEvent(() => { // Event should never change
                SetCraftable(c);
                AudioManager.Instance.PlaySound(choiceClickSound);
            });

            craftableChoices.Add(craftablChoice);
        }
        // Height of ScrollView
        var sizeDelta = craftableListRect.sizeDelta;
        sizeDelta.y = 
            (Game.Instance.gameData.GetCraftableAmount() - 1) * 50f + 
            Game.Instance.gameData.GetCraftableAmount() * -3;
        craftableListRect.sizeDelta = sizeDelta;

        RemoveCraftable();
    }

    public void SetCraftable(Craftable c) {
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

            atomName.Append(a.GetName() + "\n");
            atomAmo.Append(atoms[i].amo + "\n");

            AtomData data = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
            atomHave.Append(data.GetCurrAmo() + "\n");
        }

        atomsRequiredText.text = atomName.ToString();
        atomAmountText.text = atomAmo.ToString();
        currAtomAmountText.text = atomHave.ToString();

        bool canCraft = Game.Instance.playerData.CanCraft(c);

        craftBtn.interactable = canCraft;
        craftBtnx10.interactable = canCraft;
        craftBtnx100.interactable = canCraft;
    }
    public void RemoveCraftable() {
        craftableImage.sprite = null;

        craftableNameText.text = "???";
        craftableInfoText.text = "Price: $\nCurr Amo: ";
        atomsRequiredText.text = "";
        atomAmountText.text = "";
        currAtomAmountText.text = "";

        craftBtn.interactable = false;
        craftBtnx10.interactable = false;
        craftBtnx100.interactable = false;
    }
    public void SetCraftableChoice(ChoiceOption choice, Craftable c) {
        int amo = Game.Instance.playerData.GetCraftableAmount(c);
        choice.SetText(c.GetName() + " (" + amo  + ")\n<size=80%>    Price: $" + c.GetPrice());
    }

    public void Craft(int amount) {
        var result = Game.Instance.playerData.Craft(currCraftable, amount);

        //if (resultsToggle.isOn) {
            resultUI.Setup(null, result.atomsUsed);
        //}

        var temp = currCraftable;
        Refresh();
        SetCraftable(temp);
    }

    public void Refresh() {
        RemoveCraftable();
        for (int i = 0; i < craftableChoices.Count; i++) {
            Craftable c = Game.Instance.gameData.GetCraftable(i);

            SetCraftableChoice(craftableChoices[i], c);
        }
    }

    private void OnEnable() {
        Refresh();
    }

}
