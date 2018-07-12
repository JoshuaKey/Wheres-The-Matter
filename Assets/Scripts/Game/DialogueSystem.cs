using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {

    [Header("Components")]
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;

    [SerializeField] RectTransform nameRect;
    [SerializeField] Image background;
    [SerializeField] Button textButton;
    [SerializeField] AudioSource dialogueSource;

    [Header("Values")]
    [SerializeField] float characterDisplayTime;

    private bool displaying = false;
    private bool skip = false;
    private Queue<string> dialogueQueue = new Queue<string>();

    public delegate void Event();

    public event Event OnDialogueEnd;

    //public static DialogueSystem Instance = null;
    //private void Awake() {
    //    Instance = this;
    //}

    private void Start() {
        this.gameObject.SetActive(false);
    }

    public void QueueDialogue(string text, bool display = false) {
        dialogueQueue.Enqueue(text);

        if (display) {
            StartDialogue();
        }
    }
    public void QueueDialogue(string[] text, bool display = false) {
        for(int i = 0; i < text.Length; i++) {
            dialogueQueue.Enqueue(text[i]);
        }

        if (display) {
            StartDialogue();
        }
    }

    public void SetCharacterImage(Sprite image) {
        characterImage.sprite = image;
    }
    public void SetCharacterName(string name) {
        nameText.text = name;

        var size = nameText.GetPreferredValues(name, Mathf.Infinity, nameRect.rect.height);
        size.y = nameRect.sizeDelta.y;
        size.x += 80f;
        nameRect.sizeDelta = size;
    }

    private void StartDialogue() {
        this.gameObject.SetActive(true);

        if (!displaying) {
            StartCoroutine(DisplayDialogue());
        }
    }

    [ContextMenu("Continue Dialogue")]
    public void ContinueDialogue() {
        if (displaying) { // Skip
            skip = true;
        }  else if (dialogueQueue.Count <= 0) { // End Dialogue
            EndDialogue();
        } else {
            StartCoroutine(DisplayDialogue()); // Continue
        }
    }
    
    public void EndDialogue() {
        if (OnDialogueEnd != null) {
            OnDialogueEnd();
        }
        this.gameObject.SetActive(false);
        print("Dialogue End");

        dialogueText.text = "";
    }

    private IEnumerator DisplayDialogue() {
        if (dialogueQueue.Count <= 0) { yield break; }


        skip = false;
        displaying = true;
        dialogueText.text = "";
        dialogueSource.Play();

        float nextDisplayTime = 0f;
        string text = dialogueQueue.Dequeue();

        for (int y = 0; y < text.Length; y++) {
            while (!skip && nextDisplayTime > Time.time) {
                yield return null;
            }

            dialogueText.text += text[y];
            nextDisplayTime = Time.time + characterDisplayTime;
        }

        displaying = false;
        dialogueSource.Stop();
    }

}
