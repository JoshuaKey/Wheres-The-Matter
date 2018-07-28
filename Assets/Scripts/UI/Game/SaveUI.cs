using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveUI : MonoBehaviour {

    struct SaveDisplay {
        public bool exists;
        public string file;
        public float time;
        public Atom maxAtom;
        public float money;
    }

    [SerializeField] private TextMeshProUGUI[] saveTexts;
    private SaveDisplay[] saves;

    [SerializeField] public RectTransform rect;
    [SerializeField] public Button backBtn;

    [Header("State Button")]
    [SerializeField] public RectTransform saveRect;
    [SerializeField] public RectTransform loadRect;
    [SerializeField] public RectTransform deleteRect;
    [SerializeField] public Button saveButton;
    [SerializeField] public Button loadButton;
    [SerializeField] public Button deleteButton;

    [Header("Prompt")]
    [SerializeField] private RectTransform promptParent;
    [SerializeField] private TextMeshProUGUI titlePrompt;
    [SerializeField] private TextMeshProUGUI textPrompt;
    [SerializeField] private Button confirmButton;

    public enum eState {
        Saving,
        Loading,
        Deleting,
    }
    private string savePath;
    private eState saveState;
    private int currSave;

    private void Start() {
        if (saves == null) {
            saves = new SaveDisplay[saveTexts.Length];
        }
        savePath = Application.dataPath + "/../Saves/";
        if(!Directory.Exists(savePath)) {
            Directory.CreateDirectory(savePath);
        }

        FindSaves();
    }

    public void FindSaves() {
        for (int i = 0; i < saveTexts.Length; i++) {
            saves[i].exists = false;
            saveTexts[i].text = "No Save";
            saveTexts[i].alignment = TextAlignmentOptions.Center;
        }

        for (int i = 0; i < saveTexts.Length; i++) {
            saves[i].file = savePath + "save" + i + ".sav";
            saves[i].exists = File.Exists(saves[i].file);
            if (saves[i].exists) {
                SaveData save = new SaveData();
                try {
                    string json = File.ReadAllText(saves[i].file);
                    save = JsonUtility.FromJson<SaveData>(json);
                } catch (IOException) {
                    continue;
                } catch (System.ArgumentException) {
                    continue;
                }

                saves[i].time = save.time;
                int hours = (int)(save.time / 3600);
                save.time -= save.time / 3600;
                int minute = (int)(save.time / 60);
                save.time -= save.time / 60;
                int second = (int)(save.time);
                // hour, minute, second
                //   3600     60          0

                saves[i].money = save.money;
                float money = save.money;
                string lastSave = save.lastSave;

                string time = hours + ":" + minute + ":" + second;
                saves[i].maxAtom = save.maxAtom;
                string atom = save.maxAtom == null ? "" : save.maxAtom.ToString();

                saveTexts[i].text = "$" + money + "<size=80%>\n" + lastSave + " " + time + 
                    "\n" + atom;
                saveTexts[i].alignment = TextAlignmentOptions.TopLeft;
            }
        }
    }

    public void NewSave() {
        string file = savePath + "save" + currSave + ".sav";
        Game.Instance.Save(file);
    }    
    public void Load() {
        string file =  savePath + "save" + currSave + ".sav";
        if (Game.Instance == null) {
            PlayerPrefs.SetString("loadFile", file);
            SceneManager.LoadScene(2);
        } else {
            Game.Instance.Load(file);
        }
    }
    public void Delete() {
        string file = savePath + "save" + currSave + ".sav";
        File.Delete(file);
    }

    public void SetState(int i) {
        saveState = (eState)i;
        if(saveState == eState.Saving) {
            saveButton.interactable = false;
            loadButton.interactable = true;
            deleteButton.interactable = true;
        } else if (saveState == eState.Loading) {
            loadButton.interactable = false;
            deleteButton.interactable = true;
            saveButton.interactable = true;
        } else { // Deleting
            saveButton.interactable = true;
            loadButton.interactable = true;
            deleteButton.interactable = false;
        }
    }
    public void SetCurrSave(int index) {
        currSave = index;
    }

    public void Prompt() {
        if (saveState == eState.Saving) {
            titlePrompt.text = "Saving";
            textPrompt.text = saves[currSave].exists ?
                "Do you want to overwrite this save?" :
                "Are you sure you want to save?";
        } else if (saveState == eState.Loading) {
            if (!saves[currSave].exists) { return; }
            titlePrompt.text = "Loading";
            textPrompt.text = "Are you sure you want to load this file?<size=60%>\nYou will lose all unsaved progress.";
        } else if (saveState == eState.Deleting) {
            if (!saves[currSave].exists) { return; }
            titlePrompt.text = "Delete";
            textPrompt.text = "Are you sure you want to delete this file?";
        }
        
        promptParent.gameObject.SetActive(true);
    }

    public void Submit() { // Prompt confirm
        if (saveState == eState.Saving) {
            NewSave();
        } else if (saveState == eState.Loading) {
            Load();
        } else if (saveState == eState.Deleting) {
            Delete();
        }
        promptParent.gameObject.SetActive(false);
        FindSaves();
    }

    public void Cancel() {
        promptParent.gameObject.SetActive(false);
    }

    private void OnEnable() {
        //if (saves == null) {
        //    saves = new SaveDisplay[saveTexts.Length];
        //}
        //FindSaves();
        //SetState(1);
    }

}
