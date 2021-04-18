using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Phi.ButtonExstensions;
[RequireComponent(typeof(Collider))]
public class ButtonTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onButtonPressed;
    private bool pressedInProgress = false;

    private void OnTriggerEnter(Collider other) { 
        if (other.isTriggerButton() && !pressedInProgress) {
            pressedInProgress = true;
            onButtonPressed?.Invoke();
            Debug.Log("Error A");
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.isTriggerButton()) {
            pressedInProgress = false;
            onButtonPressed?.Invoke();
            Debug.Log("Error B");
        }
    }
}
