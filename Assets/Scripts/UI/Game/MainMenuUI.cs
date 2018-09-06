using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    [SerializeField] int currScene;
    [SerializeField] int gameScene;
    [SerializeField] SettingsUI settingsUI;

    [Header("Scene Stuff")]
    [SerializeField] RectTransform menuScreen;
    [SerializeField] SaveUI saveUI;
    [SerializeField] RectTransform loadingScreen;
    [SerializeField] TextMeshProUGUI percentText;

    [Header("Cursor")]
    [SerializeField] Cursor cursor;

    private AsyncOperation operation;
    private float timePercent;

    private void Start() {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;

        saveUI.saveRect.gameObject.SetActive(false);
        StartCoroutine(LoadScene(gameScene, currScene)); // Takes a while to build...
    }

    private void Update() {
        cursor.UpdatePosition();
    }

    public void NewGame() {
        menuScreen.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
    }
    public void PlayGame() {
        if (operation != null) {
            operation.allowSceneActivation = true;
        }
    }

    public void DisplaySave() {
        saveUI.gameObject.SetActive(true);
    }
    public void HideSave() {
        saveUI.gameObject.SetActive(false);
    }
    public void DisplaySettings() {
        settingsUI.gameObject.SetActive(true);
    }
    public void HideSettings() {
        settingsUI.gameObject.SetActive(false);
    }

    public void Exit() {
        Application.Quit();
        Debug.Break();
    }

    private IEnumerator LoadScene(int newScene, int oldScene) {
        operation = SceneManager.LoadSceneAsync(newScene);
        operation.allowSceneActivation = false;

        percentText.gameObject.SetActive(true);
        timePercent = 0f;

        while (!operation.isDone) {
            timePercent += Time.deltaTime;


            if (operation.progress >= .9f) {
                percentText.text = "Click to Continue";
            } else {
                percentText.text = "Loading " + (int)(operation.progress / .9f * 100) + "%";
            }

            yield return null;
        }
    }
}
