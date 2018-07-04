using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElementDisplay : MonoBehaviour {

    public Atom atom;
    
    [SerializeField] private Button clickButton;
    [SerializeField] private EventTrigger trigger;

    [SerializeField] private Image background;
    [SerializeField] private Image lowBackground;
    [SerializeField] private Image exclamationImage;
    [SerializeField] private TextMeshProUGUI atomNumber;
    [SerializeField] private TextMeshProUGUI atomAbbreviationText;

    [Header("Other")]
    [SerializeField] private AudioClip clickSound;

    private bool hasBeenDiscovered = false;

    public void Start() {
        this.gameObject.name = atom.GetName() + "Display";
        SetDisplay();
        SetEvents();
    }

    public void SetDisplay() {
        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        Atom display = atom;
        if (!info.IsDiscovered()) {
            info = Game.Instance.gameData.GetUknownInfo();
            display = Game.Instance.gameData.GetUknown();
        } else if(!hasBeenDiscovered) {
            exclamationImage.gameObject.SetActive(true);
            hasBeenDiscovered = true;
        }

        // Display Stuff
        atomNumber.text = display.GetAtomicNumber() == -1 ? "" : "" + display.GetAtomicNumber();
        atomAbbreviationText.text = display.GetAbbreviation();

        Color c = info.GetCategoryColor();
        background.color = c;
        c *= .6f;
        c.a = 1f;
        lowBackground.color = c;
    }

    public void SetEvents() {
        SetDisplay();

        // Event Stuff
        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(() => {
            MakeOld();
            ElementsPage.Instance.ClickAtom(atom);
            AudioManager.Instance.PlaySound(clickSound);
        });
        
        EventTrigger.Entry pointerEnterEvent = new EventTrigger.Entry();
        pointerEnterEvent.eventID = EventTriggerType.PointerEnter;
        pointerEnterEvent.callback.AddListener((x) => { ElementsPage.Instance.HoverAtom(atom); });

        EventTrigger.Entry pointerExitEvent = new EventTrigger.Entry();
        pointerExitEvent.eventID = EventTriggerType.PointerExit;
        pointerExitEvent.callback.AddListener((x) => { ElementsPage.Instance.UnHoverAtom(atom); });

        trigger.triggers.Add(pointerEnterEvent);
        trigger.triggers.Add(pointerExitEvent);
    }

    public void MakeOld() {
        exclamationImage.gameObject.SetActive(false);

        // Delete stuff to free up Data?
        // Can Change Click Event to stop CPU usage...
    }

}
