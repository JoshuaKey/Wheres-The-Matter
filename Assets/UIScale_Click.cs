using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScale_Click : MonoBehaviour {

    [SerializeField] RectTransform rect;
    [SerializeField] MainMenu_AtomEffect atomEffect;

    public void Click() {
        var sizeDelta = rect.sizeDelta;

        if (sizeDelta.x < 100) {
            sizeDelta.x *= 1.1f;
            sizeDelta.y *= 1.1f;
        }

        rect.sizeDelta = sizeDelta;

        atomEffect.Check();
    }

}
