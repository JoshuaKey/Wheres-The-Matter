using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    [SerializeField] int tutorialScene;
    [SerializeField] int gameScene;
    [SerializeField] int helpScene;

    private void Start() {
        UnityEngine.Cursor.visible = true;
    }

    public void PlayTutorial() {
        SceneManager.LoadScene(tutorialScene); // This destorys everything in the current scene... Ok.
    }
    public void PlayGame() {
        SceneManager.LoadScene(gameScene); // Takes a while to build...
    }
    public void PlayHelp() {
        SceneManager.LoadScene(helpScene);
    }
    public void LoadGame() {
        print("No Load UI");
    }
    public void Exit() {
        Application.Quit();
        Debug.Break();
    }
}
