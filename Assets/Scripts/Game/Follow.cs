using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float speed;
    [SerializeField] bool exact;
	
	// Update is called once per frame
	void LateUpdate () {
        if (exact) {
            Vector3 pos = target.position - offset;

            this.transform.position = pos;
        } else {
            Vector3 distance = target.position - (this.transform.position + offset);
            Vector3 direction = distance.normalized;

            this.transform.Translate(direction * speed * Time.deltaTime);
        }
	}
}
