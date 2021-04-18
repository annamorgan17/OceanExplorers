using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class JoyStickControll : MonoBehaviour
{
    public Transform topOfJoystick;
    [SerializeField] private float forwardBackwardsTilt = 0;
    [SerializeField] private float sideToSideTilt = 0;
    [SerializeField] private UnityEvent OnLeverUp;
    [SerializeField] private bool leverMode = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        forwardBackwardsTilt = topOfJoystick.root.eulerAngles.x; 
        if (forwardBackwardsTilt < 355 && forwardBackwardsTilt > 290) { 
            forwardBackwardsTilt = Mathf.Abs(forwardBackwardsTilt - 360);
            //Debug.Log("Backwards" + forwardBackwardsTilt); 
        } else if (forwardBackwardsTilt > 5 && forwardBackwardsTilt < 74){ 
            //Debug.Log("Foward" + forwardBackwardsTilt); 
        } 
        if (transform.eulerAngles.x >= 275) { 
            OnLeverUp?.Invoke();
        }

        if (!leverMode) { //dont bother debuging and calculating side values if we are not using them
            sideToSideTilt = topOfJoystick.rotation.eulerAngles.z; 
            if (sideToSideTilt < 355 && sideToSideTilt > 290) { 
                sideToSideTilt = Mathf.Abs(sideToSideTilt - 360);
                Debug.Log("Right " + sideToSideTilt); 
            } else if (sideToSideTilt > 5 && sideToSideTilt < 74) { 
                Debug.Log("Left " + sideToSideTilt); 
            } 
            if (sideToSideTilt < 355 && sideToSideTilt > 290) { 
                sideToSideTilt = Mathf.Abs(sideToSideTilt - 360);
                Debug.Log("Right " + sideToSideTilt); 
            } else if (sideToSideTilt > 5 && sideToSideTilt < 74) { 
                Debug.Log("Left " + sideToSideTilt); 
            }
        }

    }
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("PlayerHand")) {
            Vector3 oldEuler = transform.eulerAngles; 
            transform.LookAt(other.transform.position, transform.up); 
            Vector3 newEuler = transform.eulerAngles;
            if (leverMode) { // limit right and left direction if it is acting like a lever rather than a joystick 
                if (newEuler.x < 30 & newEuler.x > 0) {
                    Debug.LogError("Was out of bounds so reset");
                    newEuler.x = 0;
                } else {
                    //Debug.LogError("ss");
                } 
                transform.eulerAngles = new Vector3(newEuler.x, oldEuler.y, newEuler.z); 
            } 
        }
    }
}
