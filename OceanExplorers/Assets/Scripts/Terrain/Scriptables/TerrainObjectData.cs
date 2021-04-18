using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class TerrainObjectData : ScriptableObject {
    [SerializeField] private List<TerrainObject> terrainList = new List<TerrainObject>();
    public GameObject flockManager = null;
    [HideInInspector] public TerrainObject getRandomObject() { 
        int randomValue = Random.Range(0, terrainList.Count);
        return terrainList[randomValue];
    }
    public TerrainObject getAboveWaterObject() {
        List<TerrainObject> AboveWater = new List<TerrainObject>();
        foreach (var item in terrainList) {
            if (item.AboveWaterLevel) {
                AboveWater.Add(item);
            }
        }
        int randomValue = Random.Range(0, AboveWater.Count);
        TerrainObject ret =  AboveWater[randomValue];
        return ret;
    }
}
[System.Serializable] public class TerrainObject {
    public GameObject objectPrefab;
    public bool surfaceSpawn = false;
    public bool randomRotation = true;
    public bool AboveWaterLevel = false;
}