using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandMat : MonoBehaviour {
    //Quick and dirty was to set controllers to upgraded materials
    //Found in the Unity forums


    public Material mat; // this is the mat you want the hands changed to.


    private GameObject handLeft;
    private GameObject handRight;

    //
    void Update() {
        // find and grab the hand objects
        handRight = GameObject.Find("hand_right_renderPart_0");
        handLeft = GameObject.Find("hand_left_renderPart_0");

        // if i've found the hands change the texture
        if (handRight != null && handLeft != null) {
            handLeft.GetComponent<Renderer>().material = mat;
            handRight.GetComponent<Renderer>().material = mat;
            Debug.LogError("kkkkkkkkk");
            Destroy(GetComponent<UpgradeHandMat>()); // remove this script so it stops running.
        }
    }
}
