using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {

    [SerializeField] int currScene;
    [SerializeField] int gameScene;
    [SerializeField] TextMeshProUGUI percentText;

    private AsyncOperation operation;
    private float timePercent;

    public void PlayGame() {
        if(operation != null) {
            operation.allowSceneActivation = true;
        }
    }

    private void OnEnable() {
        StartCoroutine(LoadScene(gameScene, currScene));
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
            }  else {
                percentText.text = (int)(operation.progress / .9f * 100) + "%";
            }

            yield return null;
        }
    }
}
