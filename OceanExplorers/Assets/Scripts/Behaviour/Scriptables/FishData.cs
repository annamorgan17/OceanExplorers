using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class FishData : ScriptableObject {
    public FishInfo[] fishInformation;
}

public class FishInfo {
    public GameObject prefrab;
    public string name;
    public string description; 
    public FishInfo() {
        this.prefrab = new GameObject("Null Fish");
        this.name = "null";
        this.description = "no data was given";
    }
    public FishInfo(GameObject prefrab,  string name, string description) {
        this.prefrab = prefrab;
        this.name = name;
        this.description = description;
    }
}
