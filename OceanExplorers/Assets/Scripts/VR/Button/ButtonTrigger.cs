using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Phi.ButtonExstensions;

//the button script that attaches to the trigger of the button
[RequireComponent(typeof(Collider))] public class ButtonTrigger : MonoBehaviour {
    [SerializeField]
    private UnityEvent onButtonPressed;
    private bool pressedInProgress = false;

    //when triggered

    private void OnTriggerEnter(Collider other) { 
        //if its a valid collider and its not been pressed
        if (other.isTriggerButton() && !pressedInProgress) {
            pressedInProgress = true;
            //invoke the pressed event
            onButtonPressed?.Invoke();
            Debug.Log("Pressed");
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.isTriggerButton()) {
            pressedInProgress = false; 
            //onButtonPressed?.Invoke();
            Debug.Log("End Press");
        }
    }
}
