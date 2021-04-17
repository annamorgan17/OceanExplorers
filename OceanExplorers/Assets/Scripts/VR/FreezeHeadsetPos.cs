using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeHeadsetPos : MonoBehaviour
{
    public GameObject Headset;

    void Enable() {
        transform.position = Headset.transform.position;
        Headset.transform.localPosition = Vector3.zero;
    }

    void LateUpdate() {
        //transform.position = Headset.transform.localPosition * -1;
    }
}
