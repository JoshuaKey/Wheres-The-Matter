using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceOption : MonoBehaviour {

    [SerializeField] Button clickBtn;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image backgroundImage;
    [SerializeField] RectTransform rect;

    public static readonly Color defaultNormalColor = Color.white;
    public static readonly Color defaultHoverColor = new Color(.65f, .65f, .65f);
    public static readonly Color defaultPressedColor = new Color(.3f, .3f, .3f);

    public void SetButtonEvent(UnityEngine.Events.UnityAction method, bool removePriouseTriggers = true) {
        if (removePriouseTriggers) {
            clickBtn.onClick.RemoveAllListeners();
        }
        clickBtn.onClick.AddListener(method);
    }
    public void SetInteractable(bool value) {
        clickBtn.interactable = value;
    }
    public void SetBaseColor(Color c) {
        var colors = clickBtn.colors;
        colors.normalColor = c;
        clickBtn.colors = colors;
    }
    public void SetClickCapture(Color c) {
        var colors = clickBtn.colors;
        colors.pressedColor = c;
        clickBtn.colors = colors;
    }
    public void SetColors(Color normal, Color hover, Color pressed) {
        var colors = clickBtn.colors;
        colors.highlightedColor = hover;
        colors.normalColor = normal;
        colors.pressedColor = pressed;
        clickBtn.colors = colors;
    }
    public void SetFocus(bool focused) {
        if (focused) {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        } else {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void SetText(string value) {
        text.text = value;
    }
    public void SetTextAlignment(TextAlignmentOptions value) {
        text.alignment = value;
    }

    public void SetHeight(float value) {
        var sizeD = rect.sizeDelta;
        sizeD.y = value;
        rect.sizeDelta = sizeD; 
    }
    public void SetColor(Color c) {
        backgroundImage.color = c;
    }
}
