using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBounds : MonoBehaviour { 
    void Update()
    {
        //find mesh bounds, was used for debug previous
        //currently unused
        Debug.Log(gameObject.GetComponent<MeshFilter>().mesh.bounds.size);
    }
}
