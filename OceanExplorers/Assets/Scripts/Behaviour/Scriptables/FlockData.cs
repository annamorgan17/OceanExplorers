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
    public int[] fishAmount = { 10, 10, 10, 10 };
    public int[] predatorsAmount = { 1, 3 }; //shark then tuna
    public int[] soloFishAmount = { 5, 5, 5 };

    public GameObject[] fishprefab = null;
    public GameObject[] soloFishprefab = null;
    public GameObject bubblePrefab = null;


}
