using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    [SerializeField] private AtomCollector hydrogenTree;

    public void Story() {
        switch (Game.Instance.story.GetChapter()) {
            case 1:
                break;
            default:
                if(hydrogenTree != null) {
                    hydrogenTree.gameObject.SetActive(false);
                    Destroy(hydrogenTree.gameObject);
                    hydrogenTree = null;
                }           
                break;
        }
    }
}
