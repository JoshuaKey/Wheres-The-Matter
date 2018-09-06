using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class SettingsUI : MonoBehaviour {

    public Button backBtn;

    [Header("Components")]
    public AudioMixer audioMixer;
    public Vector2 cursorSpeedScale;
    public Vector2 masterVolScale;
    public Vector2 musicVolScale;
    public Vector2 soundVolScale;

    [Header("Panels")]
    public RectTransform gamePage;
    public RectTransform creditsPage;

    [Header("UI")]
    public TMP_Dropdown resolutionChoice;
    public TMP_Dropdown graphicsChoice;

    public Toggle fullScreenToggle;
    public Toggle rightHandToggle;

    public Slider cursorSlider;
    public TextMeshProUGUI cursorValue;

    public Slider masterSlider;
    public TextMeshProUGUI masterValue;

    public Slider musicSlider;
    public TextMeshProUGUI musicValue;

    public Slider soundSlider;
    public TextMeshProUGUI soundValue;

    static Resolution[] resolutions = null;

    private void Awake() {
        if (resolutions == null) {
            resolutions = Screen.resolutions;
            resolutionChoice.ClearOptions();

            //int currRes = 0;
            List<string> resoTest = new List<string>();

            for (int i = 0; i < resolutions.Length; i++) {
                resoTest.Add(resolutions[i].width + "x" + resolutions[i].height);
            }

            resolutionChoice.AddOptions(resoTest);
        }
    }

    public void DisplayGameSettings() {
        creditsPage.gameObject.SetActive(false);
        gamePage.gameObject.SetActive(true);
    }
    public void DisplayCredits() {
        creditsPage.gameObject.SetActive(true);
        gamePage.gameObject.SetActive(false);
    }

    public void SetResolution(int index) {
        // Variable...
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
    public void SetGraphics(int index) {
        // Very Low, Low, Medium, High, Very High, Ultra
        QualitySettings.SetQualityLevel(index);
    }
    public void SetFullscreen(bool isFull) {
        Screen.SetResolution(Screen.width, Screen.height, isFull);
    }
    public void SetRightHanded(bool val) {
        Player.isRightHanded = val;
        if(Game.Instance != null) {
            Game.Instance.player.IsRightHanded(Player.isRightHanded);
        }
    }

    public void SetCursorSensitivity(float val) {
        cursorValue.text = "" + Mathf.Round(val * 100);
        Cursor.cursorSpeed = Mathf.Lerp(cursorSpeedScale.x, cursorSpeedScale.y, val);
    }

    public void SetMasterVolume(float val) {
        masterValue.text = "" + Mathf.Round(val * 100);
        audioMixer.SetFloat("MasterVol", Mathf.Lerp(masterVolScale.x, masterVolScale.y, val));
    }
    public void SetMusicVolume(float val) {
        musicValue.text = "" + Mathf.Round(val * 100);
        audioMixer.SetFloat("MusicVol", Mathf.Lerp(musicVolScale.x, musicVolScale.y, val));
    }
    public void SetSoundVolume(float val) {
        soundValue.text = "" + Mathf.Round(val * 100);
        audioMixer.SetFloat("SoundVol", Mathf.Lerp(soundVolScale.x, soundVolScale.y, val));
    }

    private void OnEnable() {
        {
            float vol;
            audioMixer.GetFloat("SoundVol", out vol);
            soundSlider.value = Mathf.InverseLerp(soundVolScale.x, soundVolScale.y, vol);
        }
        {
            float vol;
            audioMixer.GetFloat("MusicVol", out vol);
            musicSlider.value = Mathf.InverseLerp(musicVolScale.x, musicVolScale.y, vol);
        }
        {
            float vol;
            audioMixer.GetFloat("MasterVol", out vol);
            masterSlider.value = Mathf.InverseLerp(masterVolScale.x, masterVolScale.y, vol);
        }

        {
            // Mouse Sensitivity?
            cursorSlider.value = Mathf.InverseLerp(cursorSpeedScale.x, cursorSpeedScale.y, Cursor.cursorSpeed);
        }

        {
           int qualityLevel = QualitySettings.GetQualityLevel();
           graphicsChoice.value = qualityLevel;
        }

        if (resolutions != null) {
            for(int i = 0; i < resolutions.Length; i++) {
                if(Screen.width == resolutions[i].width &&
                    Screen.height == resolutions[i].height) {
                    resolutionChoice.value = i;
                    break;
                }
            }
            resolutionChoice.RefreshShownValue();
        }

        {
            fullScreenToggle.isOn = Screen.fullScreen;
        }

        {
            rightHandToggle.isOn = Player.isRightHanded;
        }
        
    }
}
