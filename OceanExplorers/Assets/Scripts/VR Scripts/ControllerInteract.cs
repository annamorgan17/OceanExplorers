//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.VR;

//public class ControllerInteract : MonoBehaviour
//{
//    private int buttonTouchID;
//    private void Update()
//    {
//        UnityEngine.Input.GetTouch(buttonTouchID);

//        if (buttonTouchID != 0)
//        {
//            Debug.Log(buttonTouchID);
//        }

//        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("h"))
//        {
//            Debug.Log("mouse press");

//            Vector3 fwd = transform.TransformDirection(Vector3.forward);

//            if (Physics.Raycast(transform.position, fwd, 10))
//            {
//                Debug.Log("work");
//            }
//        }
//    }
//}