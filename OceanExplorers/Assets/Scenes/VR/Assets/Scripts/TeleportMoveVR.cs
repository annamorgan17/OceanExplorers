using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class TeleportMoveVR : MonoBehaviour
{
    private string joystickName;
    private string[] joystick;

    private void Start()
    {
        joystick = UnityEngine.Input.GetJoystickNames();
        Debug.Log(joystick);
    }

    private void Update()
    {
        for(int i = 0; i < joystick.Length; i++)
        {
            joystickName = joystick[i];
            Debug.Log(joystickName);
        }

        UnityEngine.Input.IsJoystickPreconfigured(joystickName);
    }
}
