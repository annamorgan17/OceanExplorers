using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuOpewn : MonoBehaviour
{
    [SerializeField] GameObject CanvasUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            CanvasUI.SetActive(!CanvasUI.activeSelf);
        }
    }
}
