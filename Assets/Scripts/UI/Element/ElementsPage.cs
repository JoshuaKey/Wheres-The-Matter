using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsPage : MonoBehaviour {

    [SerializeField] private Image background;
    [SerializeField] public ElementSection elementSection;
    [SerializeField] private ElementHover elementHover;
    [SerializeField] private ElementPage elementPage;
    [SerializeField] private RectTransform infoPage;

    public static ElementsPage Instance = null;
    public void Start() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    public void ClickAtom(Atom a) {
        elementPage.Setup(a);
        elementPage.Display();
    }
    public void HoverAtom(Atom a) {
        elementHover.gameObject.SetActive(true);
        elementHover.Setup(a);
    }
    public void UnHoverAtom(Atom a) {
        //if(a.GetAtomicNumber() == elementHover.GetAtom().GetAtomicNumber()) {
        //    elementHover.gameObject.SetActive(false);
        //}
    }

    public void ClickInfo() {
        infoPage.gameObject.SetActive(true);
    }
    public void HideInfo() {
        infoPage.gameObject.SetActive(false);
    }

    private void OnEnable() {
        elementSection.Refresh();

        elementHover.gameObject.SetActive(false);
    }
}

// 18 columns, 9 Rows
// 924 x 519
// Width = 51
// Height = 57 -> 51 (Even)

// The elementsPage will have an ElementSection and a MenuSection

// The MenuSection will self organize the Menu with buttons

// The ElementSection will self organize and Elements with ElementDisplays. It will also use ElementHover
// ElementDisplay should show the Element Image, Name and Curr Amo AT LEAST. It also functions as a button.
// ElementHover should show more AtomData about the atom. It should activate when hovering over the Atom.

// Clicking an ElementDisplay brings up ElementPage.
// ElementPage should showcase all information of an Atom.

// Right now, it's hard to fit all the bare information. Maybe it's better if we put it on elemenetHover?
// Get rid of Atomic Number? Name is important...
// What if instead of hover, We expanded the 