using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScale_Click : MonoBehaviour {

    [SerializeField] AudioClip clickSound;
    [SerializeField] RectTransform rect;
    [SerializeField] MainMenu_AtomEffect atomEffect;

    public void Click() {
        if (transform.localScale.x > 2) { return; }

        var scale = transform.localScale;

        scale.x *= 1.1f;
        scale.y *= 1.1f;
        scale.z *= 1.1f;

        transform.localScale = scale;

        AudioManager.Instance.PlaySound(clickSound);
        atomEffect.Check();
    }

}
