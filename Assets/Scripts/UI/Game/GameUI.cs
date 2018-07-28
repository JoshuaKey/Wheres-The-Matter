using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    [SerializeField] public RectTransform rect;
    [SerializeField] public RectTransform moneyRect;
    [SerializeField] public TextMeshProUGUI moneyText;
    [SerializeField] public Button menuButton;

    private void Start() {
        Game.Instance.playerData.OnMoneyChange += OnMoneyChange;
    }

    private void OnMoneyChange(float value) {
        string text = "$" + value;

        var size = moneyText.GetPreferredValues(text, Mathf.Infinity, moneyRect.rect.height);
        size.y = moneyRect.sizeDelta.y;
        size.x = Mathf.Abs(size.x) + 30f;
        moneyRect.sizeDelta = size;

        moneyText.text = text;
    }
}
