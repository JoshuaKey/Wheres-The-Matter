using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogSystem : MonoBehaviour {

    [Header("UI")]
    [SerializeField] RectTransform thisRect;
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] Image background;

    [Header("Data")]
    [SerializeField] Color textEndColor;
    [SerializeField] Color imageEndColor;
    [SerializeField] int lineCap;
    [SerializeField] float timeToWait;
    [SerializeField] float timeToFade;
    [SerializeField] float comboTime;

    private Color textStartColor;
    private Color imageStartColor;

    private bool fading;
    private float lastFadeTime;
    [SerializeField] private List<string> lines = new List<string>();

    private void Start() {
        textStartColor = logText.color;
        imageStartColor = background.color;

        logText.text = "";

        this.gameObject.SetActive(false);
    }

    public void Log(string line) {
        this.gameObject.SetActive(true);

        float lastTime = Time.time - lastFadeTime;
        if(lastTime > comboTime) { lines.Clear(); }

        lines.Add(line);
        if(lines.Count > lineCap) {
            lines.RemoveRange(lineCap, lines.Count - lineCap);
        }

        string text = "";
        //for (int i = lines.Count - 1; i >= 0; i--) {
        //    text += lines[i] + "\n";
        //}
        for (int i = 0; i < lines.Count; i++) {
            text += lines[i] + "\n";
        }
        logText.text = text;

        var size = logText.GetPreferredValues(text);
        size.x = Mathf.Abs(size.x) + 20;
        size.y = Mathf.Abs(size.y) + 20;
        thisRect.sizeDelta = size;


        if (fading) {
            StopAllCoroutines();
        }
        lastFadeTime = Time.time;
        StartCoroutine(Fade());
    }

    private IEnumerator Fade() {
        fading = true;

        logText.color = textStartColor;
        background.color = imageStartColor;

        float endTime = Time.time + timeToWait;
        while (Time.time < endTime) {
            yield return null;
        }

        endTime = Time.time + timeToFade;
        while(Time.time < endTime) {
            float t = 1 - (endTime - Time.time) / timeToFade;

            Color c = Color.Lerp(textStartColor, textEndColor, t);
            logText.color = c;

            c = Color.Lerp(imageStartColor, imageEndColor, t);
            background.color = c;

            yield return null;
        }

        lastFadeTime = Time.time;
        fading = false;
        this.gameObject.SetActive(false);
    }

    public void OnClick() {
        StopAllCoroutines();

        lastFadeTime = Time.time;

        fading = false;
        this.gameObject.SetActive(false);
    }

    private void OnDisable() {
        OnClick();
    }

}
