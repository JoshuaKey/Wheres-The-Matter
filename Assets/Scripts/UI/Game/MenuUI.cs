using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour {

    [SerializeField] public Button resumeBtn;
    [SerializeField] public Button elementBtn;
    [SerializeField] public Button labBtn;
    [SerializeField] public Button mapBtn;
    [SerializeField] public Button saveBtn;
    [SerializeField] public Button settingBtn;
    [SerializeField] public Button exitBtn;
    [SerializeField] public Prompt prompt;
    [SerializeField] public AudioClip buttonClick;

    public void Exit() {
        prompt.gameObject.SetActive(true);

        prompt.headerText.text = "Exit";
        prompt.promptText.text = "Do you want to exit?<size=60%>\nYou will lose all unsaved progress.";

        prompt.confirmBtn.onClick.RemoveAllListeners();
        prompt.confirmBtn.onClick.AddListener(() => {
            AudioManager.Instance.PlaySound(buttonClick);
            Game.Instance.Exit();
        });

        prompt.cancelBtn.onClick.RemoveAllListeners();
        prompt.cancelBtn.onClick.AddListener(() => {
            AudioManager.Instance.PlaySound(buttonClick);
            prompt.gameObject.SetActive(false);
        });
    }
}
