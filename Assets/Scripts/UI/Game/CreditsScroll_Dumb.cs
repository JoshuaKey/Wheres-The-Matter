using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll_Dumb : MonoBehaviour {

    public Scrollbar dumbScrollBar;

    private void OnEnable() {
        dumbScrollBar.value = 1;
    }
}
