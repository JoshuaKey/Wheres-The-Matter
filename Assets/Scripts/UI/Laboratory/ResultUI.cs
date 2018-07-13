using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour {

    [Header("UI")]
    [SerializeField] private RectTransform usedAtoms;
    [SerializeField] private TextMeshProUGUI usedAtomName;
    [SerializeField] private TextMeshProUGUI usedAtomAmo;
    [SerializeField] private Scrollbar usedScrollbar;

    [SerializeField] private RectTransform producedAtoms;
    [SerializeField] private TextMeshProUGUI producedAtomName;
    [SerializeField] private TextMeshProUGUI producedAtomAmo;
    [SerializeField] private Scrollbar producedScrollbar;

    [Header("Other")]
    [SerializeField] private AudioClip successClip;

    public delegate void OnClick();
    public event OnClick OnResultUIClick;

    public void Setup(List<AtomAmo> results, List<AtomAmo> used) {
        this.gameObject.SetActive(true);

        // Used Atoms
        if (used != null) {
             
            if (used.Count > 0 && used[0].atom != null) {
                usedAtomName.text = used[0].atom.GetName()+"\n";
                usedAtomAmo.text = "" + used[0].amo + "\n";
            }
            for (int i = 1; i < used.Count; i++) {
                if (used[i].atom != null) {
                    usedAtomName.text += used[i].atom.GetName() + "\n";
                    usedAtomAmo.text += used[i].amo + "\n";
                }
            }

            var size = usedAtoms.sizeDelta;
            size.y = 36 * used.Count;
            usedAtoms.sizeDelta = size;

            size = usedAtomName.rectTransform.sizeDelta;
            size.y = 36 * used.Count;
            usedAtomName.rectTransform.sizeDelta = size;

            size = usedAtomAmo.rectTransform.sizeDelta;
            size.y = 36 * used.Count;
            usedAtomAmo.rectTransform.sizeDelta = size;
        } else {

            var size = usedAtoms.sizeDelta;
            size.y = 0;
            usedAtoms.sizeDelta = size;

            usedAtomName.text = "";
            usedAtomAmo.text = "";
        }

        // Produced Atoms
        if (results != null) {
            if (results.Count > 0 && results[0].atom != null) {
                producedAtomName.text = results[0].atom.GetName() + "\n";
                producedAtomAmo.text = "" + results[0].amo + "\n";
            }
            for (int i = 1; i < results.Count; i++) {
                if (results[i].atom != null) {
                    producedAtomName.text += results[i].atom.GetName() + "\n";
                    producedAtomAmo.text += results[i].amo + "\n";
                }
            }

            var size = producedAtoms.sizeDelta;
            size.y = 36 * results.Count;
            producedAtoms.sizeDelta = size;

            size = producedAtomName.rectTransform.sizeDelta;
            size.y = 36 * results.Count;
            producedAtomName.rectTransform.sizeDelta = size;

            size = producedAtomAmo.rectTransform.sizeDelta;
            size.y = 36 * results.Count;
            producedAtomAmo.rectTransform.sizeDelta = size;
        } else {
            var size = producedAtoms.sizeDelta;
            size.y = 0;
            producedAtoms.sizeDelta = size;

            producedAtomName.text = "";
            producedAtomAmo.text = "";
        }

        usedScrollbar.value = 1;
        producedScrollbar.value = 1;

        AudioManager.Instance.PlaySound(successClip, .8f);
    }

    public void Click() {
        if(OnResultUIClick != null) {
            OnResultUIClick();
        }

        this.gameObject.SetActive(false);
    }
}
