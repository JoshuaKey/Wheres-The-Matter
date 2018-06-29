using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour {

    [SerializeField] private RectTransform resultRect;
    [SerializeField] private AtomResult atomResultPrefab;
    [SerializeField] private AudioClip successClip;

    [SerializeField] private TextMeshProUGUI usedText; // Shouldnt Change
    [SerializeField] private TextMeshProUGUI producedText;

    private List<AtomResult> atomResults = new List<AtomResult>();

    public void Setup(List<AtomAmo> results, List<AtomAmo> used) {
        this.gameObject.SetActive(true);

        float height = usedText.transform.localPosition.y; // Start at Used Text
        height -= usedText.rectTransform.sizeDelta.y*2;

        int i = 0;
        for (; i < used.Count; i++) {
            AtomResult result = null;
            AtomAmo atomAmo = used[i];

            if (atomResults.Count <= i) {
                result = Instantiate(atomResultPrefab, resultRect);
                result.transform.localScale = Vector3.one;

                atomResults.Add(result);
            } else {
                result = atomResults[i];
            }
            var pos = result.transform.localPosition;
            pos.y = height;
            result.transform.localPosition = pos;
            height -= 60f;

            result.gameObject.SetActive(true);

            AtomInfo info = Game.Instance.gameData.FindAtomInfo(atomAmo.atom.GetAtomicNumber());

            result.atomImage.sprite = info.GetImage();
            result.atomName.text = atomAmo.atom.GetName();
            result.atomAmo.text = "" + atomAmo.amo;
        }

        if(results != null && results.Count != 0) {
            // Produced Text
            { 
                var pos = producedText.transform.localPosition;
                pos.y = height;
                producedText.transform.localPosition = pos;

                height -= producedText.rectTransform.sizeDelta.y * 2;

                producedText.gameObject.SetActive(true);
            }

            // Atoms
            for (int y = 0; y < results.Count; y++, i++) {
                AtomResult result = null;
                AtomAmo atomAmo = results[y];

                if (atomResults.Count <= i) {
                    result = Instantiate(atomResultPrefab, resultRect);
                    result.transform.localScale = Vector3.one;

                    atomResults.Add(result);
                } else {
                    result = atomResults[i];
                }
                var pos = result.transform.localPosition;
                pos.y = height;
                result.transform.localPosition = pos;
                height -= 60f;

                result.gameObject.SetActive(true);

                AtomInfo info = Game.Instance.gameData.FindAtomInfo(atomAmo.atom.GetAtomicNumber());

                result.atomImage.sprite = info.GetImage();
                result.atomName.text = atomAmo.atom.GetName();
                result.atomAmo.text = "" + atomAmo.amo;
            }
        } else {
            producedText.gameObject.SetActive(false);
        }


        for(; i < atomResults.Count; i++) {
            atomResults[i].gameObject.SetActive(false);
        }

        AudioManager.Instance.PlaySound(successClip, .8f);
    }

    public void Disable() {
        this.gameObject.SetActive(false);
    }
}
