using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour {

    [Header("UI")]
    [SerializeField] private RectTransform usedAtoms;
    [SerializeField] private TextMeshProUGUI usedAtomName;
    [SerializeField] private TextMeshProUGUI usedAtomAmo;

    [SerializeField] private RectTransform producedAtoms;
    [SerializeField] private TextMeshProUGUI producedAtomName;
    [SerializeField] private TextMeshProUGUI producedAtomAmo;

    [Header("Other")]
    [SerializeField] private AudioClip successClip;

    public void Setup(List<AtomAmo> results, List<AtomAmo> used) {
        this.gameObject.SetActive(true);

        // Used Atoms
        if (used != null) {
             
            if (used.Count > 0) {
                usedAtomName.text = used[0].atom.GetName()+"\n";
                usedAtomAmo.text = "" + used[0].amo + "\n";
            }
            for (int i = 1; i < used.Count; i++) {
                usedAtomName.text += used[i].atom.GetName() + "\n";
                usedAtomAmo.text += used[i].amo + "\n";
            }

            var size = usedAtoms.sizeDelta;
            size.y = 36 * used.Count;
            usedAtoms.sizeDelta = size;
            print("Used: " + size);
        } else {

            var size = usedAtoms.sizeDelta;
            size.y = 0;
            usedAtoms.sizeDelta = size;
        }

        // Produced Atoms
        if (results != null) {
            if (results.Count > 0) {
                producedAtomName.text = results[0].atom.GetName() + "\n";
                producedAtomAmo.text = "" + results[0].amo + "\n";
            }
            for (int i = 1; i < results.Count; i++) {
                producedAtomName.text += results[i].atom.GetName()+"\n";
                producedAtomAmo.text += results[i].amo+"\n";
            }

            var size = producedAtoms.sizeDelta;
            size.y = 36 * results.Count;
            producedAtoms.sizeDelta = size;
        } else {
            var size = producedAtoms.sizeDelta;
            size.y = 0;
            producedAtoms.sizeDelta = size;
        }
       
        AudioManager.Instance.PlaySound(successClip, .8f);
    }

    public void Disable() {
        this.gameObject.SetActive(false);
    }
}
