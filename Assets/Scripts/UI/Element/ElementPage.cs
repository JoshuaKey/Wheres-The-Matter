using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementPage : MonoBehaviour {

    private Atom atom;
    [SerializeField] ElementSection elementSection;

    [Header("Display Info")]
    [SerializeField] Image atomImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI atomicNumberText;

    [Header("Data Info")]
    [SerializeField] TextMeshProUGUI currAmoText;
    [SerializeField] TextMeshProUGUI passiveGainText;
    [SerializeField] TextMeshProUGUI totalCollectedText;
    [SerializeField] TextMeshProUGUI totalUsedText;

    [Header("Description Info")]
    [SerializeField] RectTransform descriptionInfo;
    [SerializeField] RectTransform descriptionContent;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Scrollbar descriptionScroll;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI weightText;
    [SerializeField] TextMeshProUGUI densityText;

    [Header("Isotope Info")]
    [SerializeField] RectTransform isotopeInfo;
    [SerializeField] TextMeshProUGUI protonsText;
    [SerializeField] TextMeshProUGUI electronsText;
    [SerializeField] TextMeshProUGUI neutronsText;
    [SerializeField] TextMeshProUGUI radioactiveText;
    [SerializeField] TextMeshProUGUI isotopeText;
    
    [Header("Area Info")]
    [SerializeField] RectTransform areaInfo;
    [SerializeField] TextMeshProUGUI placesText;
    [SerializeField] TextMeshProUGUI originsText;

    public void Setup(Atom a) {
        atom = a;

        AtomData data = Game.Instance.gameData.FindAtomData(atom.GetAtomicNumber());
        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        if (!info.IsDiscovered()) {
            info = Game.Instance.gameData.GetUknownInfo();
            data = Game.Instance.gameData.GetUknownData();
            a = Game.Instance.gameData.GetUknown();
        }

        // Display
        nameText.text = a.GetName();
        atomicNumberText.text = "Atomic Number: \n" + (a.GetAtomicNumber() == -1 ? "?" : "" + a.GetAtomicNumber());
        atomImage.sprite = info.GetImage();

        // Data
        currAmoText.text = "Current Amo: " + data.GetCurrAmo();
        passiveGainText.text = "Passive Gain: " + data.GetPassiveGain(); // What to do with this???
        totalCollectedText.text = "Total Atoms: " + data.GetTotalCollected();
        totalUsedText.text = "Total Used: " + data.GetTotalUsed();

        // Description
        var size = descriptionText.GetPreferredValues(info.GetDescription(), descriptionText.rectTransform.rect.width, Mathf.Infinity);
        size.x = descriptionContent.sizeDelta.x;
        size.y = Mathf.Abs(size.y) + 5;
        descriptionContent.sizeDelta = size;
        //descriptionText.rectTransform.sizeDelta = size;

        descriptionText.text = info.GetDescription();
        descriptionScroll.value = 1;
        typeText.text = "Type: " + info.GetCategoryString();
        weightText.text = "Weight: " + (info.GetWeight() == -1 ? "???" : "" +info.GetWeight());
        densityText.text = "Density: " + (info.GetDensity() == -1 ? "???" : 
            info.GetDensity() + " g/cm<sup>3</sup>");

        // Isotope
        protonsText.text = "P: " + (info.GetProtons() == -1 ? "?" : ""+ info.GetProtons());
        electronsText.text = "E: " + (info.GetElectrons() == -1 ? "?" : "" + info.GetElectrons());
        neutronsText.text = "N: " + (info.GetNeutrons() == -1 ? "?" : "" + info.GetNeutrons());
        radioactiveText.text = "Is Radioactive: " + !info.IsStable();// + " " + info.GetHalfLife() + " " + info.GetStability();

        string isotopeString = "Isotopes:\n<size=80%>\t";
        var isotopes = info.GetIsotopes();
        if (isotopes.Length > 0) {
            isotopeString += isotopes[0] + "\n\t";
        }
        for (int i = 1; i < isotopes.Length /*&& i < 7*/; i++) {  // MAximum of 7 isotopes???
            isotopeString += isotopes[i] + "\n\t";
        }
        isotopeText.text = isotopeString;

        // Area
        string placesString = "Places:\n";
        var places = info.GetPlacesToBeFound();
        if (places.Length > 0) {
            placesString += "  " + places[0] + "\n";
        }
        for(int i = 1; i < places.Length; i++) {
            placesString += "  " + places[i] + "\n";
        }
        placesText.text = placesString;

        string originsString = "Origins:\n";
        var origins = info.GetOrigins();
        if (origins.Length > 0) {
            originsString += "  " + origins[0] + "\n";
        }
        for (int i = 1; i < origins.Length; i++) {
            originsString += "  " + origins[i] + "\n";
        }
        originsText.text = originsString;
    }

    public void FlipInfo(int pageNum) {
        switch (pageNum) {
            case 1:
                isotopeInfo.gameObject.SetActive(true);
                areaInfo.gameObject.SetActive(false);
                descriptionInfo.gameObject.SetActive(false);
                break;
            case 2:
                isotopeInfo.gameObject.SetActive(false);
                areaInfo.gameObject.SetActive(true);
                descriptionInfo.gameObject.SetActive(false);
                break;
            default:
                isotopeInfo.gameObject.SetActive(false);
                areaInfo.gameObject.SetActive(false);
                descriptionInfo.gameObject.SetActive(true);
                break;

        }
    }

    public void NextElement() {
        if(this.atom != null) {
            int number = atom.GetAtomicNumber() + 1;
            if(number > Game.Instance.gameData.GetAtomAmount()) {
                number = 1;
            }
            Atom a = Game.Instance.gameData.FindAtom(number);
            //Setup(a);
            ElementsPage.Instance.ClickAtom(a);
        }
    }
    public void PrevElement() {
        if (this.atom != null) {
            int number = atom.GetAtomicNumber() - 1;
            if (number < 1) {
                number = Game.Instance.gameData.GetAtomAmount();
            }
            Atom a = Game.Instance.gameData.FindAtom(number);
            //Setup(a);
            ElementsPage.Instance.ClickAtom(a);
        }
    }

    public void Display() {
        this.gameObject.SetActive(true);
    }
    public void Close() { 
        this.gameObject.SetActive(false);
    }
}
