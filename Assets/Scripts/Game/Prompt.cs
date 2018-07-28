using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour {

    [SerializeField] public Button cancelBtn;
    [SerializeField] public Button confirmBtn;

    [SerializeField] public TextMeshProUGUI headerText;
    [SerializeField] public TextMeshProUGUI promptText;
}
