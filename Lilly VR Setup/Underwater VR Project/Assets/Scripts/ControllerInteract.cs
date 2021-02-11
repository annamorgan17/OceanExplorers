using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class ControllerInteract : MonoBehaviour
{
    private int buttonTouchID;
    private void Update()
    {
        UnityEngine.Input.GetTouch(buttonTouchID);

        if (buttonTouchID != 0)
        {
            Debug.Log(buttonTouchID);
        }
    }
}
