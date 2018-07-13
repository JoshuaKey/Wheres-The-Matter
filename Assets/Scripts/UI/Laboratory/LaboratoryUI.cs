using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaboratoryUI : MonoBehaviour {

    [SerializeField] RectTransform upgradeUI; // Turn into Scroll with Info...
    [SerializeField] RectTransform combineUI;
    [SerializeField] RectTransform splitUI;
    [SerializeField] RectTransform craftUI;
    [SerializeField] ResultUI resultUI;
    [SerializeField] RenameUI renameUI;

    public void DisplayUpgrades() {
        upgradeUI.gameObject.SetActive(true);
    }
    public void DisplayCombine() {
        combineUI.gameObject.SetActive(true);
    }
    public void DisplaySplit() {
        splitUI.gameObject.SetActive(true);
    }
    public void DisplayCraft() {
        craftUI.gameObject.SetActive(true);
    }

    private void OnEnable() {
        Reset();
    }

    public void Reset() {
        combineUI.gameObject.SetActive(false);
        splitUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        craftUI.gameObject.SetActive(false);

        resultUI.gameObject.SetActive(false);
        renameUI.gameObject.SetActive(false); 
    }
}
