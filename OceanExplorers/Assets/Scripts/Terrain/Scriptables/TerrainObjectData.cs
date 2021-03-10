using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class TerrainObjectData : ScriptableObject {
    [SerializeField] private TerrainObject[] terrainList = new TerrainObject[1];
    [HideInInspector] public TerrainObject getRandomObject() { 
        int randomValue = Random.Range(0, terrainList.Length);
        return terrainList[randomValue];
    }
}
[System.Serializable] public class TerrainObject {
    public GameObject objectPrefab;
    public bool surfaceSpawn = false; 
}