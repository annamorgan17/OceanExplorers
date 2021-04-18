using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a vr button
public class ButtonLimit : MonoBehaviour {
    [SerializeField] private GameObject buttonTrigger;
    private Vector3 orgionalPosition;
    private float minDistance;
    private float maxDistance;

    private void Awake() {
        //calculate the min and max distance
        minDistance = Vector3.Distance(buttonTrigger.transform.position, transform.position);
        maxDistance = buttonTrigger.transform.position.y;
        orgionalPosition = transform.position;
    }

    private void Update() {
        //if the button is bellow the min distance set it back to the min
        bool BellowMinDistance = Vector3.Distance(buttonTrigger.transform.position, transform.position) >= minDistance;
        if (BellowMinDistance) {
            transform.position = orgionalPosition;

        }

        //if the button is above the max distance, set it back to the max
        bool AboveMaxDistance = transform.position.y <= maxDistance;
        if (AboveMaxDistance) {
            transform.position = new Vector3(transform.position.x, maxDistance, transform.position.z);
        }
    }
}
