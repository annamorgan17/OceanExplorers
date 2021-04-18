using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class GetInput : MonoBehaviour
{
    [SerializeField] Canvas LibraryCanvas;
    [SerializeField] Canvas OptionCanvas;
    [SerializeField] selectingObjScript scan; 

    void Update() {
        //get button inputs and do appropirate event
        if (OVRInput.GetDown(OVRInput.Button.Back)) { // b button
            Debug.Log("Throw some countdown to return to main menu");
            //if on menu exit game
        }
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger)) { // right side button
            ScanFish(); // scan fish
        } 
        if (OVRInput.GetDown(OVRInput.RawButton.Y)) { // z button
            ShowMenu(); // show fish library
        }
        if (Input.GetKeyDown(KeyCode.Escape) ){
            OptionCanvas.gameObject.SetActive(!OptionCanvas.gameObject.activeSelf); // show options menu on pc mode only
        }
    }
    private void ScanFish() {
        Debug.Log("Scan a fish");
        //scan.CheckObjectClicked();
    }
    private void ShowMenu() { 
        Debug.Log("Show Fish Library menu");
        LibraryCanvas.gameObject.SetActive(!LibraryCanvas.gameObject.activeSelf);
        LibraryCanvas.transform.position = gameObject.transform.position;
        LibraryCanvas.transform.rotation = gameObject.transform.rotation;
        LibraryCanvas.transform.Translate(gameObject.transform.right * 5f); 
    }
}
