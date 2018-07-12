using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RenameUI : MonoBehaviour {

    [SerializeField] TextMeshProUGUI elementNameText;
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField abbrField;
    [SerializeField] TextMeshProUGUI infoText;

    [Header("Button")]
    [SerializeField] Button finishedBtn;
    [SerializeField] TextMeshProUGUI finishedText;

    [Header("Other")]
    [SerializeField] AudioClip clickSound;
    [SerializeField] private AudioClip successClip;

    private Atom atom;

    public delegate void OnClick();
    public event OnClick OnRenameClick;

    public void Setup(Atom atom) {
        if (!atom.CanBeRenamed()) { return; }
        this.atom = atom;

        elementNameText.text = "Element " + atom.GetAtomicNumber();

        nameField.text = atom.GetName();
        abbrField.text = atom.GetAbbreviation();

        OnSelect();

        this.gameObject.SetActive(true);
    }

    public void OnSelect() {
        infoText.text = "This name can not be changed";
        finishedText.text = "Done";

        finishedBtn.onClick.RemoveAllListeners();
        finishedBtn.onClick.AddListener(() => Check());

        if(nameField.text == "" || abbrField.text == "") {
            finishedBtn.interactable = false;
        } else {
            finishedBtn.interactable = true;
        }
    }

    public void Check() {
        AudioManager.Instance.PlayUISound(clickSound);
        infoText.text = "Are you Sure?";

        finishedText.text = "<color=\"red\">Submit";

        finishedBtn.onClick.RemoveAllListeners();
        finishedBtn.onClick.AddListener(() => Submit());

        EventSystem.current.SetSelectedGameObject(null);
    }
    public void Submit() {
        AudioManager.Instance.PlayUISound(successClip);

        string name = nameField.text;
        string abbr = abbrField.text;
        abbr = char.ToUpper(abbr[0]) + abbr.Substring(1).ToLower();

        Game.Instance.playerData.RenameAtom(atom.GetAtomicNumber(), name, abbr);

        this.gameObject.SetActive(false);

        if (OnRenameClick != null) {
            OnRenameClick();
        }
    }
}
