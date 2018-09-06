using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cursor : MonoBehaviour {

    [SerializeField] float size;

    [Header("Components")]
    [SerializeField] private Image image;
    [SerializeField] private RectTransform thisRect;
    [SerializeField] private Camera mainCamera;

    static public float cursorSpeed = 1.0f; // Where the flip do I store this???

    // Use this for initialization
    public void Start () {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
	}

    public void SetImage(Sprite s) {
        image.sprite = s;
    }

    public Vector3 GetWorldPosition() {
        return mainCamera.ScreenToWorldPoint(thisRect.position);
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

    public bool IsOverUIElement() {
        return EventSystem.current.IsPointerOverGameObject();
    }
    public bool HasSellectedUIElement() {
        return EventSystem.current.currentSelectedGameObject != null;
    }
}
