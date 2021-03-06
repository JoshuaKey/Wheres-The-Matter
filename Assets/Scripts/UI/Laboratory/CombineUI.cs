﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombineUI : MonoBehaviour {

    [Header("UI")]
    [SerializeField] Slider atomAAmo;
    [SerializeField] Slider atomBAmo;
    [SerializeField] Image atomAImage;
    [SerializeField] Image atomBImage;
    [SerializeField] Button atomABtn;
    [SerializeField] Button atomBBtn;
    [SerializeField] Image atomResultImage;
    [SerializeField] Button produceButton;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI atomAText;
    [SerializeField] TextMeshProUGUI atomBText;
    [SerializeField] TextMeshProUGUI atomResultText;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI atomAAmoText;
    [SerializeField] TextMeshProUGUI atomBAmoText;

    [Header("Other")]
    [SerializeField] RectTransform atomList;
    [SerializeField] ChoiceOption choicePrefab;
    [SerializeField] AudioClip choiceClickSound;
    [SerializeField] ResultUI resultUI;
    [SerializeField] RenameUI renameUI;

    private List<ChoiceOption> atomChoices = new List<ChoiceOption>();
    private List<Atom> discoveredAtoms = new List<Atom>();

    private float startPos = 0;
    private float yPos = 0;

    private Atom atomA;
    private Atom atomB;
    private Atom atomProduced;

    private void Start() {
        //float startPos = -5;
        //float yPos = startPos;
        for (int i = 0; i < Game.Instance.gameData.GetAtomAmount(); i++) {
            Atom a = Game.Instance.gameData.FindAtom(i + 1);

            var atomChoice = Instantiate(choicePrefab);

            // Pos
            atomChoice.transform.SetParent(atomList, false);

            atomChoice.transform.localScale = Vector3.one;

            //var pos = atomChoice.transform.localPosition;
            //pos.y = yPos;
            //atomChoice.transform.localPosition = pos;
            //yPos -= 50f;

            // Data / Functionality
            var data = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
            string text = a.GetName() + "\n<size=80%> Atomic Number: " + a.GetAtomicNumber() + " Curr Amo: " + data.GetCurrAmo();
            atomChoice.SetText(text);
            atomChoice.SetButtonEvent(() => { // Event should never change
                SetAtom(a);
                AudioManager.Instance.PlaySound(choiceClickSound);
            });
            atomChoice.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);

            atomChoices.Add(atomChoice);

            //AtomData data = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
            if (data.IsDiscovered()) {
                discoveredAtoms.Add(a);
            } else {
                atomChoice.gameObject.SetActive(false);
            }
        }
        // Height of ScrollView
        //var sizeDelta = atomList.sizeDelta;
        //sizeDelta.y = startPos - yPos + 10;
        //atomList.sizeDelta = sizeDelta;

        renameUI.OnRenameClick += Reset;
        SortElements();
        Game.Instance.gameData.OnAtomDiscover += OnAtomDiscover;
    }

    private void OnAtomDiscover(Atom a, float amo) {
        AddElement(a, atomChoices[a.GetAtomicNumber() - 1]);
    }
    private void AddElement(Atom a, ChoiceOption c) {
        if (discoveredAtoms.Contains(a)) {
            return;
        }

        c.gameObject.SetActive(true);
        discoveredAtoms.Add(a);
        SortElements();
    }
    private void SortElements() {
        discoveredAtoms.Sort();

        startPos = -5;
        yPos = startPos;
        for (int i = 0; i < discoveredAtoms.Count; i++) {
            ChoiceOption c = atomChoices[discoveredAtoms[i].GetAtomicNumber() - 1];

            var pos = c.transform.localPosition;
            pos.y = yPos;
            c.transform.localPosition = pos;
            yPos -= 50;
        }

        var sizeDelta = atomList.sizeDelta;
        sizeDelta.y = startPos - yPos + 10;
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
        if (atom != null && atomChoices.Count > atom.GetAtomicNumber()) {
            ChoiceOption choiceOption = atomChoices[atom.GetAtomicNumber() - 1];
            choiceOption.SetButtonEvent(() => {
                RemoveAtomA();
                AudioManager.Instance.PlaySound(choiceClickSound);
            });
            choiceOption.SetColors(ChoiceOption.defaultPressedColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultNormalColor);
            choiceOption.SetFocus(false);
        }
        atomA = atom;
        //atomChoices[atomA.GetAtomicNumber() - 1].SetInteractable(false);

        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        AtomData data = Game.Instance.gameData.FindAtomData(atom.GetAtomicNumber());

        atomAText.text = info.GetAtom().GetName();
        atomAImage.sprite = info.GetImage();

        atomAAmo.maxValue = data.GetCurrAmo();
        SetAtomAAmoText();

        atomABtn.interactable = true;
    }
    public void SetAtomB(Atom atom) {
        if (atom != null && atomChoices.Count > atom.GetAtomicNumber()) {
            ChoiceOption choiceOption = atomChoices[atom.GetAtomicNumber() - 1];
            choiceOption.SetButtonEvent(() => {
                RemoveAtomB();
                AudioManager.Instance.PlaySound(choiceClickSound);
            });
            choiceOption.SetColors(ChoiceOption.defaultPressedColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultNormalColor);
            choiceOption.SetFocus(false);
        }
        atomB = atom;
        //atomChoices[atomB.GetAtomicNumber() - 1].SetInteractable(false);

        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        AtomData data = Game.Instance.gameData.FindAtomData(atom.GetAtomicNumber());

        atomBText.text = info.GetAtom().GetName();
        atomBImage.sprite = info.GetImage();

        atomBAmo.maxValue = data.GetCurrAmo();
        SetAtomBAmoText();

        atomBBtn.interactable = true;
    }

    public void RemoveAtomA() {
        //if (atomA != null) {
        //    atomChoices[atomA.GetAtomicNumber() - 1].SetInteractable(true);
        //}
        if (atomA != null && atomChoices.Count > atomA.GetAtomicNumber()) {
            ChoiceOption choiceOption = atomChoices[atomA.GetAtomicNumber() - 1];
            var atom = atomA;
            choiceOption.SetButtonEvent(() => {
                SetAtom(atom);
                AudioManager.Instance.PlaySound(choiceClickSound);
            });
            choiceOption.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);
            choiceOption.SetFocus(false);
        }
        atomA = null;

        AtomInfo info = Game.Instance.gameData.GetUknownInfo();
        atomAText.text = info.GetAtom().GetName();
        atomAImage.sprite = info.GetImage();

        produceButton.interactable = false;
        infoText.text = "";
        atomResultImage.sprite = info.GetImage();
        atomResultText.text = info.GetAtom().GetName();
        atomAAmoText.text = "";
        atomAAmo.value = 0;

        atomABtn.interactable = false;
    }
    public void RemoveAtomB() {
        //if (atomB != null) {
        //    atomChoices[atomB.GetAtomicNumber() - 1].SetInteractable(true);
        //}
        if (atomB != null && atomChoices.Count > atomB.GetAtomicNumber()) {
            ChoiceOption choiceOption = atomChoices[atomB.GetAtomicNumber() - 1];
            var atom = atomB;
            choiceOption.SetButtonEvent(() => {
                SetAtom(atom);
                AudioManager.Instance.PlaySound(choiceClickSound);
            });
            choiceOption.SetColors(ChoiceOption.defaultNormalColor, ChoiceOption.defaultHoverColor, ChoiceOption.defaultPressedColor);
            choiceOption.SetFocus(false);
        }
        atomB = null;

        AtomInfo info = Game.Instance.gameData.GetUknownInfo();
        atomBText.text = info.GetAtom().GetName();
        atomBImage.sprite = info.GetImage();

        produceButton.interactable = false;
        infoText.text = "";
        atomResultImage.sprite = info.GetImage();
        atomResultText.text = info.GetAtom().GetName();
        atomBAmoText.text = "";
        atomBAmo.value = 0;

        atomBBtn.interactable = false;
    }

    private void CalculateInfo() {
        var info = Game.Instance.playerData.EstimateCombine(atomA, atomB, (int)atomAAmo.value, (int)atomBAmo.value);
        if(info.targetAtom == null) {
            infoText.text = "That atom can not be created.";
            produceButton.interactable = false;
            atomResultText.text = Game.Instance.gameData.GetUknown().GetName();
            return;
        }


        //AtomInfo atomInfo = Game.Instance.gameData.FindAtomInfo(info.targetAtom.GetAtomicNumber());
        AtomData atomData = Game.Instance.gameData.FindAtomData(info.targetAtom.GetAtomicNumber());
        if (!atomData.IsDiscovered()) {
            atomResultText.text = Game.Instance.gameData.GetUknown().GetName();
            atomResultImage.sprite = Game.Instance.gameData.GetUknownInfo().GetImage();
        } else {

            atomResultText.text = info.targetAtom.GetName();
            atomResultImage.sprite = Game.Instance.gameData.FindAtomInfo(info.targetAtom.GetAtomicNumber())
                .GetImage();
        }

        atomProduced = info.targetAtom;

        infoText.text = "Max Production: " + info.amo + "\nSuccess: " + info.success*100 + "% Stability: " + info.stability*100 + "%"; // Max Production, Success, Stability

        produceButton.interactable = info.amo > 0;
    }

    public void Produce() {
        var result = Game.Instance.playerData.ProduceCombine(atomA, atomB, (int)atomAAmo.value, (int)atomBAmo.value);

        if(result.atomsProduced.Count > 0 && result.atomsProduced[0].amo > 0 && result.atomsProduced[0].atom.CanBeRenamed()) {
            resultUI.OnResultUIClick += DisplayRename;
            atomProduced = result.atomsProduced[0].atom;
        }

        // Display Popup
        resultUI.Setup(result.atomsProduced, result.atomsUsed);
        Reset();

        for (int i = 0; i < result.atomsProduced.Count; i++) {
            print("Atoms Produced: " + result.atomsProduced[i].atom.GetName() + " " + result.atomsProduced[i].amo);
        }
    }

    private void DisplayRename() {
        renameUI.Setup(atomProduced);

        resultUI.OnResultUIClick -= DisplayRename;
    }

    private void OnEnable() {
        Reset();
    }

    public void Reset() {
        RemoveAtomA();
        RemoveAtomB();
        for(int i = 0; i < atomChoices.Count; i++) {
            Atom a = Game.Instance.gameData.FindAtom(i + 1);
            var data = Game.Instance.gameData.FindAtomData(a.GetAtomicNumber());
            string text = a.GetName() + "\n<size=80%> Atomic Number: " + a.GetAtomicNumber() + " Curr Amo: " + data.GetCurrAmo();
            atomChoices[i].SetText(text);
        }
    }

}
