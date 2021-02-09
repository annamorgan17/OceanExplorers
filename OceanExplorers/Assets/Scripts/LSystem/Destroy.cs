using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour { 
    //simple class to destroy objects within a class where monobehaviour is not present
    void Start() {
        Destroy(gameObject);
    }
}
