using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementHover : MonoBehaviour {

    private Atom atom;
    [SerializeField] Image atomImage;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI currAmoText;
    [SerializeField] TextMeshProUGUI passiveGainText;
    [SerializeField] TextMeshProUGUI atomicNumberText;

    [SerializeField] float offsetMultiplier = 1.1f;

    private RectTransform rect;
    private RectTransform parentRect;

    // Use this for initialization
    void Start () {
        rect = GetComponent<RectTransform>();
        parentRect = this.transform.parent.GetComponent<RectTransform>();

        Setup(atom);
	}
	
    public void Setup(Atom a) {
        atom = a;

        AtomData data = Game.Instance.gameData.FindAtomData(atom.GetAtomicNumber());
        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        if (!info.IsDiscovered()) {
            info = Game.Instance.gameData.GetUknownInfo();
            data = Game.Instance.gameData.GetUknownData();
            a = Game.Instance.gameData.GetUknown();
        }

        nameText.text = a.GetName();
        atomicNumberText.text = "" + a.GetAtomicNumber();
        //atomImage.sprite = info.GetImage();

        currAmoText.text = "Current Amount: " + data.GetCurrAmo();
        passiveGainText.text = "Passive Gain: " + data.GetPassiveGain();

        if(rect != null) {
            Update();
        }
    }

    private void Update() {
        // Right now it sort of works. 
        // Some of the math is funky and position is incorrect. 
        // Canvas seems to be offset a little which mmesses with Mouse Position
        // Pivot doesnt seem to be working

        Vector2 size = rect.rect.size * (rect.lossyScale * offsetMultiplier);
        float xDelta = size.x / 2f;
       
        Vector3 position = Input.mousePosition;

        if (position.x + size.x > (parentRect.rect.max.x - parentRect.rect.min.x) * parentRect.lossyScale.x) {
            position.x -= xDelta;
        } else {
            position.x += xDelta;
        }

        rect.position = position; // Working
    }

    public Atom GetAtom() {
        return atom;
    }
}
