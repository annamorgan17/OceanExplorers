using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class GetInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (OVRInput.GetDown(OVRInput.Button.Back)) { // b button
            Debug.Log("Throw some countdown to return to main menu");
            //if on menu exit game
        }
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger)) { // right side button
            Debug.Log("Scan a fish");
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X)) { // z button
            Debug.Log("Show Option menu");
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y)) { // z button
            Debug.Log("Show Option menu");
        }
    }
}
