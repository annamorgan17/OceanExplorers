using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class PossonData : ScriptableObject {
    public float radius = 1;
    public Vector2 sampleRegionSize = Vector2.one;
    public int numSamplesBeforeRejection = 30;
    public GameObject Prefrab;
}
