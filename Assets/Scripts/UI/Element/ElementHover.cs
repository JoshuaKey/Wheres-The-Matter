using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementHover : MonoBehaviour {

    private Atom atom;
    [SerializeField] Image background;
    [SerializeField] Image darkBackground;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI currAmoText;
    [SerializeField] TextMeshProUGUI passiveGainText;

    [SerializeField] float offsetMultiplier = 1.1f;

    private RectTransform rect;
    private RectTransform parentRect;

    private void Awake() {
        rect = GetComponent<RectTransform>();
        parentRect = this.transform.parent.GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start () {
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

        // Color
        Color c = info.GetCategoryColor();
        background.color = c;
        c *= .6f;
        c.a = 1f;
        darkBackground.color = c;

        var sizeDelta = rect == null ? Vector2.zero : rect.sizeDelta;

        // Name Text
        {
            var size = nameText.GetPreferredValues(a.GetName(), Mathf.Infinity, nameText.rectTransform.rect.height);
            size.y = nameText.rectTransform.sizeDelta.y;
            size.x += 15f;
            if (size.x > sizeDelta.x) {
                sizeDelta.x = size.x;
            }

            nameText.text = a.GetName();
        }

        // Curr Amo Text
        {
            string text = "Amount:\n " + data.GetCurrAmo();

            var size = currAmoText.GetPreferredValues(text, Mathf.Infinity, currAmoText.rectTransform.rect.height);
            size.y = currAmoText.rectTransform.sizeDelta.y;
            size.x += 15f;
            if (size.x > sizeDelta.x) {
                sizeDelta.x = size.x;
            }

            currAmoText.text = text;
        }

        // Passive Gain Text
        {
            string text = "(+" + data.GetCurrAmo() + ")";

            var size = passiveGainText.GetPreferredValues(text, Mathf.Infinity, passiveGainText.rectTransform.rect.height);
            size.y = passiveGainText.rectTransform.sizeDelta.y;
            size.x += 15f;
            if (size.x > sizeDelta.x) {
                sizeDelta.x = size.x;
            }

            passiveGainText.text = text;
        }

        rect.sizeDelta = sizeDelta;
        LateUpdate();
    }

    private void LateUpdate() {
        Vector2 size = rect.rect.size * (rect.lossyScale * offsetMultiplier);
        float xDelta = size.x / 2f;

        Vector3 position = Input.mousePosition;

        if (position.x + size.x > (parentRect.rect.max.x - parentRect.rect.min.x) * parentRect.lossyScale.x) {
            position.x -= xDelta;
        } else {
            position.x += xDelta;
        }

        rect.position = position;
    }

    public Atom GetAtom() {
        return atom;
    }
}
