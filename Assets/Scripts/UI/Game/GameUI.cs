using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour {

    [SerializeField] RectTransform moneyRect;
    [SerializeField] TextMeshProUGUI moneyText;


    private void Start() {
        Game.Instance.playerData.OnMoneyChange += OnMoneyChange;
    }

    private void OnMoneyChange(float value) {
        print("Here");
        string text = "$" + value;

        var size = moneyText.GetPreferredValues(text, Mathf.Infinity, moneyRect.rect.height);
        size.y = moneyRect.sizeDelta.y;
        size.x = Mathf.Abs(size.x) + 30f;
        moneyRect.sizeDelta = size;

        moneyText.text = text;
    }
}
