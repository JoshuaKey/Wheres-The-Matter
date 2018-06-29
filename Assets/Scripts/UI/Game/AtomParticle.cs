using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AtomParticle : MonoBehaviour {

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [HideInInspector] public RectTransform rect;
    [HideInInspector] public Atom atom;
    [HideInInspector] public int amo;

    public void Setup(Atom a, int amount, Vector3 pos) {
        atom = a;
        amo = amount;
        text.text = atom.GetAbbreviation();

        var info = Game.Instance.gameData.FindAtomInfo(a.GetAtomicNumber());
        image.color = info.GetCategoryColor();

        if(rect == null) {
            rect = GetComponent<RectTransform>();
        }

        rect.position = pos;

        this.gameObject.SetActive(true);
    }
}
