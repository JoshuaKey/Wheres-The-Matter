using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AtomDiscovery : MonoBehaviour {

    public Atom atom;
	[SerializeField] private TextMeshProUGUI atomName;
    [SerializeField] private TextMeshProUGUI atomInfo;
    [SerializeField] private Image atomImage;

    public void Setup(Atom a) {
        this.gameObject.SetActive(true);

        atom = a;

        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());

        atomName.text = atom.GetName();
        atomImage.sprite = info.GetImage();

        string placesString = "";
        var places = info.GetPlacesToBeFound();
        if (places.Length > 0) {
            placesString += "  " + places[0] + "\n";
        }
        for (int i = 1; i < places.Length; i++) {
            placesString += "  " + places[i] + "\n";
        }

        atomInfo.text = "Atomic Number: " + atom.GetAtomicNumber() + "\nAbbreviaton: " + atom.GetAbbreviation() + 
            "\n\nCategory: " + info.GetCategoryString()  + "\nNeutrons: " + info.GetNeutrons() + "\nIsRadioactive: " + !info.IsStable() + "\nWhere To Find:\n" + placesString;
    }
    public void Disable() {
        this.gameObject.SetActive(false);
    }
}
