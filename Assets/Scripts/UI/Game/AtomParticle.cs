using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AtomParticle : MonoBehaviour {

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] public RectTransform rect;
    [HideInInspector] public Atom atom;
    [HideInInspector] public int amo;

    private float life = 0f;
    private bool appearing = false;
    private bool moving = false;

    private void Start() {
        // Clamp
        if (Game.Instance != null && Game.Instance.worldCanvas != null) {
            float scale = Mathf.Clamp(30 / Game.Instance.worldCanvas.scaleFactor, 20, 30);
            var sizeD = rect.sizeDelta;
            sizeD.x = scale;
            sizeD.y = scale;
            rect.sizeDelta = sizeD;
        }
    }

    public void Setup(Atom a, int amount, Vector3 pos) {
        atom = a;
        amo = amount;
        text.text = atom.GetAbbreviation();

        var info = Game.Instance.gameData.FindAtomInfo(a.GetAtomicNumber());
        image.color = info.GetCategoryColor();

        rect.position = pos;

        this.gameObject.SetActive(true);

        life = 0f;
        appearing = true;
        moving = false;

        this.transform.localScale = Vector3.zero;
    }

    public bool UpdateParticle(Vector3 target, float collectDist, float scaleSpeed, float moveSpeed) {
        life += Time.deltaTime;

        if (appearing) {
            var scale = Vector3.Lerp(this.transform.localScale, Vector3.one, scaleSpeed * Time.deltaTime);

            this.transform.localScale = scale;

            if(scale.sqrMagnitude >= 2.9f) {
                appearing = false;
                moving = true;
                this.transform.localScale = Vector3.one;
            }
        } else if (moving) {
            var scale = Vector3.Lerp(this.transform.position, target, moveSpeed * life * Time.deltaTime);

            this.transform.position = scale;

            Vector3 distance = target - this.transform.position;
            if (distance.sqrMagnitude < collectDist) {
                moving = false;
            }
        } else {
            var scale = Vector3.Lerp(this.transform.localScale, Vector3.zero, scaleSpeed * Time.deltaTime);

            this.transform.localScale = scale;

            if (scale.sqrMagnitude <= .01f) {
                this.transform.localScale = Vector3.zero;
                return true;
            }
        }

        return false;
    }
}
