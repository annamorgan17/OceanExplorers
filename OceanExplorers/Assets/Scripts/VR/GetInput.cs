using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class GetInput : MonoBehaviour
{
    [SerializeField] Canvas LibraryCanvas;
    [SerializeField] Canvas OptionCanvas;
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
            ScanFish();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X)) { // z button
            ShowMenu(true);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y)) { // z button
            ShowMenu(false);
        }
    }
    private void ScanFish() {
        Debug.Log("Scan a fish");
    }
    private void ShowMenu(bool MenuA) {
        if (MenuA) {
            Debug.Log("Show Option menu");
        } else {
            Debug.Log("Show Fish Library menu");
        }
    }
}
