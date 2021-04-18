using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuOpewn : MonoBehaviour
{
    [SerializeField] GameObject CanvasUI;
    [SerializeField] KeyCode activator = KeyCode.Escape;
    

    // a simple script the hide the canvas when the player walks too far away from it
    
     
    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(activator)) {
            CanvasUI.SetActive(!CanvasUI.activeSelf);
            CanvasUI.transform.position = gameObject.transform.position;
            CanvasUI.transform.rotation = gameObject.transform.rotation;
            CanvasUI.transform.Translate(CanvasUI.transform.forward * 5f);
        }

        if (CanvasUI.activeSelf) {
            if (Vector3.Distance(CanvasUI.transform.position, gameObject.transform.position) >= 20f) { // remove if the character walked to far away
                CanvasUI.SetActive(false);
            } 
        }
    }
}
