using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickControll : MonoBehaviour
{
    public Transform topOfJoystick;
    [SerializeField] private float forwardBackwardsTilt = 0;
    [SerializeField] private float sideToSideTilt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        forwardBackwardsTilt = topOfJoystick.root.eulerAngles.x;

        if (forwardBackwardsTilt < 355 && forwardBackwardsTilt > 290) {

            forwardBackwardsTilt = Mathf.Abs(forwardBackwardsTilt - 360);
            Debug.Log("Backwards" + forwardBackwardsTilt);

        } else if (forwardBackwardsTilt > 5 && forwardBackwardsTilt < 74){

            Debug.Log("Foward" + forwardBackwardsTilt);

        }

        sideToSideTilt = topOfJoystick.rotation.eulerAngles.z;

        if (sideToSideTilt < 355 && sideToSideTilt > 290) {

            sideToSideTilt = Mathf.Abs(sideToSideTilt - 360);
            Debug.Log("Right " + sideToSideTilt);

        } else if (sideToSideTilt > 5 && sideToSideTilt < 74) {

            Debug.Log("Left " + sideToSideTilt);

        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("PlayerHand")) {
            transform.LookAt(other.transform.position, transform.up);
        }
    }
}
