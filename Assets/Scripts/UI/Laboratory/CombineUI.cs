using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombineUI : MonoBehaviour {

    [SerializeField] Slider atomAAmo;
    [SerializeField] Slider atomBAmo;
    [SerializeField] Image atomAImage;
    [SerializeField] Image atomBImage;
    [SerializeField] Image atomResultImage;
    [SerializeField] Button produceButton;
    [SerializeField] ResultUI resultUI;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI atomAText;
    [SerializeField] TextMeshProUGUI atomBText;
    [SerializeField] TextMeshProUGUI atomResultText;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI atomAAmoText;
    [SerializeField] TextMeshProUGUI atomBAmoText;

    [Header("Atom Choices")]
    [SerializeField] RectTransform atomList;
    [SerializeField] AtomChoice atomChoicePrefab;
    private List<AtomChoice> atomChoices = new List<AtomChoice>();

    private Atom atomA;
    private Atom atomB;

    private void Start() {
        for(int i = 0; i < Game.Instance.gameData.GetAtomAmount(); i++) {
            Atom a = Game.Instance.gameData.FindAtom(i + 1);

            var atomChoice = Instantiate(atomChoicePrefab);
            atomChoice.atom = a;
            atomChoice.combineUI = this;

            // Pos
            atomChoice.transform.SetParent(atomList,false);

            atomChoice.transform.localScale = Vector3.one;

            var pos = atomChoice.transform.localPosition;
            pos.y -= 5 + i * 50f;
            atomChoice.transform.localPosition = pos;

            atomChoices.Add(atomChoice);
        }
        var sizeDelta = atomList.sizeDelta;
        sizeDelta.y = (Game.Instance.gameData.GetAtomAmount()-1) * 50f + Game.Instance.gameData.GetAtomAmount() * -3;
        atomList.sizeDelta = sizeDelta;
    }

    public void SetAtom(Atom atom) {
        if(atomA == null) {
            SetAtomA(atom);
        } else if(atomB == null) {
            SetAtomB(atom);
        }
        // Do Nothing

        if(atomA != null && atomB != null) {
            CalculateInfo();
        }
    }

    public void SetAtomAAmoText() {
        atomAAmoText.text = "" +atomAAmo.value;
        if (atomA != null && atomB != null) {
            CalculateInfo();
        }
    }
    public void SetAtomBAmoText() {
        atomBAmoText.text = "" + atomBAmo.value;
        if (atomA != null && atomB != null) {
            CalculateInfo();
        }
    }

    public void SetAtomA(Atom atom) {
        atomA = atom;
        atomChoices[atomA.GetAtomicNumber() - 1].trigger.interactable = false;

        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        AtomData data = Game.Instance.gameData.FindAtomData(atom.GetAtomicNumber());

        atomAText.text = info.GetAtom().GetName();
        //atomAImage.sprite = info.GetImage();

        atomAAmo.maxValue = data.GetCurrAmo();
        SetAtomAAmoText();
    }
    public void SetAtomB(Atom atom) {
        atomB = atom;
        atomChoices[atomB.GetAtomicNumber() - 1].trigger.interactable = false;

        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        AtomData data = Game.Instance.gameData.FindAtomData(atom.GetAtomicNumber());

        atomBText.text = info.GetAtom().GetName();
        //atomBImage.sprite = info.GetImage();

        atomBAmo.maxValue = data.GetCurrAmo();
        SetAtomBAmoText();
    }

    public void RemoveAtomA() {
        if (atomA != null) {
            atomChoices[atomA.GetAtomicNumber() - 1].trigger.interactable = true;
        }
        atomA = null;

        AtomInfo info = Game.Instance.gameData.GetUknownInfo();
        atomAText.text = info.GetAtom().GetName();
        //atomAImage.sprite = info.GetImage();

        produceButton.interactable = false;
        infoText.text = "";
        //atomResultImage.sprite = info.GetImage();
        atomResultText.text = info.GetAtom().GetName();
        atomAAmoText.text = "";
        atomAAmo.value = 0;
    }
    public void RemoveAtomB() {
        if (atomB != null) {
            atomChoices[atomB.GetAtomicNumber() - 1].trigger.interactable = true;
        }
        atomB = null;

        AtomInfo info = Game.Instance.gameData.GetUknownInfo();
        atomBText.text = info.GetAtom().GetName();
        //atomBImage.sprite = info.GetImage();

        produceButton.interactable = false;
        infoText.text = "";
        //atomResultImage.sprite = info.GetImage();
        atomResultText.text = info.GetAtom().GetName();
        atomBAmoText.text = "";
        atomBAmo.value = 0;
    }


    private void CalculateInfo() {
        var info = Game.Instance.playerData.EstimateCombine(atomA, atomB, (int)atomAAmo.value, (int)atomBAmo.value);
        atomResultText.text = info.targetAtom.GetName();
        infoText.text = "Max Production: " + info.amo + "\nSuccess: " + info.success*100 + "% Stability: " + info.stability*100 + "%"; // Max Production, Success, Stability

        produceButton.interactable = info.amo > 0;
    }

    public void Produce() {
        //var info = Game.Instance.playerData.Produce(atomA, atomB);
        var result = Game.Instance.playerData.ProduceCombine(atomA, atomB, (int)atomAAmo.value, (int)atomBAmo.value);

        // Display Popup
        resultUI.Setup(result.atomsProduced, result.atomsUsed);
        Reset();

        for (int i = 0; i < result.atomsProduced.Count; i++) {
            print("Atoms Produced: " + result.atomsProduced[i].atom.GetName() + " " + result.atomsProduced[i].amo);
        }
    }

    private void OnEnable() {
        Reset();
    }

    public void Reset() {
        RemoveAtomA();
        RemoveAtomB();
        for(int i = 0; i < atomChoices.Count; i++) {
            atomChoices[i].SetDisplay();
        }
    }

}
