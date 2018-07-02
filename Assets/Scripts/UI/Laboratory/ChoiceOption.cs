﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceOption : MonoBehaviour {

    [SerializeField] Button clickBtn;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image backgroundImage;
    [SerializeField] RectTransform rect;

    public void SetButtonEvent(UnityEngine.Events.UnityAction method, bool removePriouseTriggers = true) {
        if (removePriouseTriggers) {
            clickBtn.onClick.RemoveAllListeners();
        }
        clickBtn.onClick.AddListener(method);
    }
    public void SetInteractable(bool value) {
        clickBtn.interactable = value;
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
