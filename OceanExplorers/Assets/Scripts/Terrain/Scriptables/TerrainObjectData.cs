using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class TerrainObjectData : ScriptableObject {
    [SerializeField] private List<TerrainObject> terrainList = new List<TerrainObject>();
    [HideInInspector] public TerrainObject getRandomObject() { 
        int randomValue = Random.Range(0, terrainList.Count);
        return terrainList[randomValue];
    }
}
[System.Serializable] public class TerrainObject {
    public GameObject objectPrefab;
    public bool surfaceSpawn = false;
    public bool randomRotation = true;
}