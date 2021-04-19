using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalObjects : MonoBehaviour
{
    void Start() {
        StartCoroutine(LateStart(1f));
    }

    IEnumerator LateStart(float waitTime) {
        Debug.Log("Late start starting");
        yield return new WaitForSeconds(waitTime);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity)) {
            Debug.Log("Raycast hit");
            //transform.position = hit.point;
        }


        //Your Function You Want to Call
    }
}
