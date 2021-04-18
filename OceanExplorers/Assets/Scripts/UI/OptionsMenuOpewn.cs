using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuOpewn : MonoBehaviour
{
    [SerializeField] GameObject CanvasUI;
    [SerializeField] KeyCode activator = KeyCode.Escape;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
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
