using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaboratoryUI : MonoBehaviour {

    [Header("UI")]
    [SerializeField] public RectTransform rect;
    [SerializeField] public UpgradeUI upgradeUI; // Turn into Scroll with Info...
    [SerializeField] public CombineUI combineUI;
    [SerializeField] public SplitUI splitUI;
    [SerializeField] public CraftUI craftUI;
    [SerializeField] public ResultUI resultUI;
    [SerializeField] public RenameUI renameUI;

    [Header("Buttons)")]
    [SerializeField] public Button backBtn;
    [SerializeField] public Button upgradeBtn;
    [SerializeField] public Button combineBtn;
    [SerializeField] public Button splitBtn;
    [SerializeField] public Button craftBtn;

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
    private void OnDisable() {
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
