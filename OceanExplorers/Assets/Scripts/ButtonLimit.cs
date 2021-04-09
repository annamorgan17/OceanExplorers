﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLimit : MonoBehaviour
{
    [SerializeField] private GameObject buttonTrigger;
    private Vector3 orgionalPosition;
    private float minDistance;
    private float maxDistance;

    private void Awake() {
        minDistance = Vector3.Distance(buttonTrigger.transform.position,
            transform.position);
        maxDistance = buttonTrigger.transform.position.y;
        orgionalPosition = transform.position;
    }

    private void Update() {
        if (Vector3.Distance(buttonTrigger.transform.position, transform.position) >= minDistance) {
            transform.position = orgionalPosition;

        }
        if (transform.position.y <= maxDistance) {
            transform.position = new Vector3(transform.position.x, maxDistance, transform.position.z);
        }
    }
}
