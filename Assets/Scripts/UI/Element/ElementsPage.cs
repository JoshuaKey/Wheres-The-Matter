﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsPage : MonoBehaviour {

    [SerializeField] private Image background;
    [SerializeField] public ElementSection elementSection;
    [SerializeField] private ElementHover elementHover;
    [SerializeField] private ElementPage elementPage;
    [SerializeField] private RectTransform infoPage;

    [Header("Button")]
    [SerializeField] private Button nextPageBtn;
    private bool nextPageUnlocked = false;

    int currPage = 1;
    bool moving = false;
    //bool hasInited = false;

    public static ElementsPage Instance = null;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    public void Start() {
        if (!Game.Instance.gameData.FindAtomInfo(119).IsDiscovered() &&
                !Game.Instance.gameData.FindAtomInfo(120).IsDiscovered() &&
                !Game.Instance.gameData.FindAtomInfo(121).IsDiscovered()) {
            nextPageBtn.gameObject.SetActive(false);
            Game.Instance.gameData.OnAtomDiscover += UnlockNextPage;
        }
    }

    public void ClickAtom(Atom a) {
        //if (a.GetAtomicNumber() >= 119) {
        //    if(nextPageUnlocked) {
        //        elementSection.ElementClick(a);
        //        elementPage.Setup(a);
        //        elementPage.Display();
        //    } else {
        //        a = Game.Instance.gameData.FindAtom(1);
        //        elementSection.ElementClick(a);
        //        elementPage.Setup(a);
        //        elementPage.Display();
        //    }
        //}  else {
        //    elementSection.ElementClick(a);
        //    elementPage.Setup(a);
        //    elementPage.Display();
        //}
        elementSection.ElementClick(a);
        elementPage.Setup(a);
        elementPage.Display();
    }
    public void HoverAtom(Atom a) {
        elementHover.gameObject.SetActive(true);
        elementHover.Setup(a);
    }
    public void UnHoverAtom(Atom a) {
        //if(a.GetAtomicNumber() == elementHover.GetAtom().GetAtomicNumber()) {
        //    elementHover.gameObject.SetActive(false);
        //}
    }

    public void ClickInfo() {
        infoPage.gameObject.SetActive(true);
    }
    public void HideInfo() {
        infoPage.gameObject.SetActive(false);
    }

    public void UnlockNextPage(Atom a, float amo) {
        if(a.GetAtomicNumber() > 118) {
            nextPageBtn.gameObject.SetActive(true);
            Game.Instance.gameData.OnAtomDiscover -= UnlockNextPage;
            nextPageUnlocked = true;
        }
    }
    public void NextPage() {
        if (currPage == 1 && !moving) {
            currPage = 2;
            var newPos = this.transform.position;
            newPos.y += Screen.height;
            StartCoroutine(Move(this.transform, newPos, .5f));
        }
    }
    public void PrevPage() {
        if(currPage == 2 && !moving) {
            currPage = 1;
            var newPos = this.transform.position;
            newPos.y -= Screen.height;
            StartCoroutine(Move(this.transform, newPos, .5f));
        }
    }

    private IEnumerator Move(Transform rect, Vector3 newPos, float time) {
        moving = true;

        Vector3 currPos = rect.position;
        float endTime = Time.time + time;
        while(Time.time < endTime) {
            float t = 1 - (endTime - Time.time) / time;

            Vector3 pos = Vector3.Lerp(currPos, newPos, t);

            rect.position = pos;

            yield return null;
        }

        //rect.position = newPos;
        moving = false;
    }

    private void OnEnable() {
        elementSection.Refresh();

        elementHover.gameObject.SetActive(false);
        elementPage.gameObject.SetActive(false);
        infoPage.gameObject.SetActive(false);
    }

    private void OnDisable() {
        //if (!hasInited && Game.Instance != null) {
        //    Game.Instance.gameData.OnAtomDiscover += UnlockNextPage;
        //    hasInited = true;
        //}
    }

    public bool IsNextPageUnlocked() { return nextPageUnlocked; }
}