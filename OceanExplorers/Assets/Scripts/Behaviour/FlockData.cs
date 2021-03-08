using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class FlockData : ScriptableObject { 
    public Vector3 swimLimits = Vector3.zero;
    public Vector3 setPoint = Vector3.zero;
   public int randomAmount = 10000;
    [Range(10.0f, 30.0f)] public float maxSpeed;
    [Range(0.5f, 10.0f)] public float fishDistance;
    [Range(5f, 10f)] public float rotationSpeed;
    [Range(2.0f, 500.0f)] public int fishAmount = 10;

    public GameObject fishprefab = null;
    public GameObject bubblesPrefab = null;
    
}
