using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class CrabData : ScriptableObject  {
    public Vector3 gameArea = Vector3.zero;
    //[SerializeField] [Range(0.1f, 5.0f)] float crabSpeed;
    //[SerializeField] [Range(1.0f, 5.0f)] float crabRotSpeed; 
    //int layerMask = 1 << 8;
    public float randomAmount = 10000;
    public int rayLength = 6; 
}
