﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SplitUI : MonoBehaviour {

    [SerializeField] Slider atomAAmo;
    [SerializeField] Image atomAImage;
    [SerializeField] Image atomResultImage;
    [SerializeField] Button splitBtn;
    [SerializeField] ResultUI resultUI;
    [SerializeField] Toggle toHydrogenToggle;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI atomAText;
    [SerializeField] TextMeshProUGUI atomResultText;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI atomAAmoText;

    [Header("Atom Choices")]
    [SerializeField] RectTransform atomList;
    [SerializeField] AtomChoice atomChoicePrefab;
    private List<AtomChoice> atomChoices = new List<AtomChoice>();

    private Atom atomA;

    private void Start() {
        for (int i = 0; i < Game.Instance.gameData.GetAtomAmount(); i++) {
            Atom a = Game.Instance.gameData.FindAtom(i + 1);

            var atomChoice = Instantiate(atomChoicePrefab);
            atomChoice.atom = a;
            atomChoice.splitUI = this;

            // Pos
            atomChoice.transform.SetParent(atomList, false);

            atomChoice.transform.localScale = Vector3.one;

            var pos = atomChoice.transform.localPosition;
            pos.y -= 5 + i * 50f;
            atomChoice.transform.localPosition = pos;

            atomChoices.Add(atomChoice);
        }
        var sizeDelta = atomList.sizeDelta;
        sizeDelta.y = (Game.Instance.gameData.GetAtomAmount() - 1) * 50f + Game.Instance.gameData.GetAtomAmount() * -3;
        atomList.sizeDelta = sizeDelta;
    }

    public void SetAtom(Atom atom) {
        if(atomA != null) {
            RemoveAtomA();
        }
        SetAtomA(atom);
        CalculateInfo();
    }

    public void SetAtomAAmoText() {
        atomAAmoText.text = "" + atomAAmo.value;
        if (atomA != null) {
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

    public void RemoveAtomA() {
        if (atomA != null) {
            atomChoices[atomA.GetAtomicNumber() - 1].trigger.interactable = true;
        }
        atomA = null;

        AtomInfo info = Game.Instance.gameData.GetUknownInfo();
        atomAText.text = info.GetAtom().GetName();
        atomAImage.sprite = info.GetImage();

        splitBtn.interactable = false;
        infoText.text = "";
        atomResultImage.sprite = info.GetImage();
        atomResultText.text = info.GetAtom().GetName();
        atomAAmoText.text = "";
        atomAAmo.value = 0;
    }

    private void CalculateInfo() {
        if(atomA.GetAtomicNumber() == 1) { // Hydrogen can not be split
            infoText.text = "Hydrogen can not be split";
            splitBtn.interactable = false;
            return;
        }
        PlayerData.AtomCollision info;
        if (toHydrogenToggle.isOn) {
            info = new PlayerData.AtomCollision();
            info.success = 1;
            info.stability = 1;

            Atom atom = atomA;
            int amo = (int)atomAAmo.value;
            do {
                var infoTemp = Game.Instance.playerData.EstimateSplit(atom, amo);
                atom = infoTemp.targetAtom;
                amo = infoTemp.amo;
                print("Currently at: " + atom.GetName() + " " + amo);

                info.targetAtom = atom;
                info.amo = amo;
                info.success *= infoTemp.success;
                info.stability *= infoTemp.stability;
            } while (atom.GetAtomicNumber() != 1);
        } else {
            info = Game.Instance.playerData.EstimateSplit(atomA, (int)atomAAmo.value);
        }

        atomResultText.text = info.targetAtom.GetName();
        infoText.text = "Max Production: " + info.amo + "\nSuccess: " + info.success * 100 + "% Stability: " + info.stability * 100 + "%"; // Max Production, Success, Stability

        splitBtn.interactable = info.amo > 0;
    }

    public void Produce() {
        PlayerData.AtomCollisionResult result;
        if (toHydrogenToggle.isOn) {
            result = new PlayerData.AtomCollisionResult();
            result.atomsProduced = new List<AtomAmo>();
            result.atomsUsed = new List<AtomAmo>();

            AtomAmo atomAmo = new AtomAmo();
            PlayerData.AtomCollisionResult resultTemp;

            Atom atom = atomA;
            int amo = (int)atomAAmo.value;
            do {
                atomAmo.atom = atom;
                atomAmo.amo = amo;
                result.atomsUsed.Add(atomAmo);

                resultTemp = Game.Instance.playerData.ProduceSplit(atom, amo);
                atom = resultTemp.atomsProduced[0].atom;
                amo = resultTemp.atomsProduced[0].amo;
            } while (atom.GetAtomicNumber() != 1);

            result.atomsProduced.Add(resultTemp.atomsProduced[0]);
        } else {
            result = Game.Instance.playerData.ProduceSplit(atomA, (int)atomAAmo.value);
        }

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
        for (int i = 0; i < atomChoices.Count; i++) {
            atomChoices[i].SetDisplay();
        }
    }
}