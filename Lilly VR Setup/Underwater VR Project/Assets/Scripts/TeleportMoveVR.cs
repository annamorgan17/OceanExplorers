using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class TeleportMoveVR : MonoBehaviour
{
    private string joystickName;
    
    private void Update()
    {
        string[] joystick = UnityEngine.Input.GetJoystickNames();
        Debug.Log(joystick);

        for(int i = 0; i < joystick.Length; i++)
        {
            joystickName = joystick[i];
            Debug.Log(joystickName);
        }

        UnityEngine.Input.IsJoystickPreconfigured(joystickName);
    }
}
