using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_AtomEffect : MonoBehaviour {

    [SerializeField] RectTransform[] rects;
    [SerializeField] AudioClip completionSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] ParticleSystem effect;
    bool completion = false;

    public void Check() {
        if (completion) { return; }

        for (int i = 0; i < rects.Length; i++) {
            var sizeDelta = rects[i].sizeDelta;
            if (sizeDelta.x < 100) {
                return;
            }
        }

        StartCoroutine(Congrats());

        AudioManager.Instance.PlaySound(completionSound);
        completion = true;
    }

    private IEnumerator Congrats() {
        float timeWait = .5f;
        for(int i = 0; i < 30f; i++) {
            var effectTemp = Instantiate(effect, this.transform);

            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(0, Screen.width);
            pos.y = Random.Range(0, Screen.height);
            pos.z = -1f;
            effectTemp.transform.position = pos;
            effectTemp.Emit(11);
            AudioManager.Instance.PlaySound(explosionSound);

            yield return new WaitForSeconds(timeWait);
            timeWait *= .95f;
        }

    }
}
