using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour {

    [SerializeField] float size;

    [Header("Components")]
    [SerializeField] private Image image;
    [SerializeField] private RectTransform thisRect;

	// Use this for initialization
	void Start () {
        //thisRect = GetComponent<RectTransform>();
        //image = GetComponent<Image>();

        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
	}

    public void SetImage(Sprite s) {
        image.sprite = s;
    }

    public void UpdatePosition() {
        thisRect.position = Input.mousePosition;
    }

    public void SetSize(float scale) {
        var sizeD = thisRect.sizeDelta;

        sizeD.x = size * scale;
        sizeD.y = size * scale;

        thisRect.sizeDelta = sizeD;
    }
    public float GetSize() {
        return thisRect.sizeDelta.x;
    }
}
