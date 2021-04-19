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
    [HideInInspector] public TerrainObject getRandomObjectNoAbove() {
        int randomValue = Random.Range(0, terrainList.Count);
        TerrainObject Out = terrainList[randomValue];
        while (Out.spawntype == SpawnType.Above) {
            randomValue = Random.Range(0, terrainList.Count);
            Out = terrainList[randomValue];
        }
        return Out;
    }
    [HideInInspector] public TerrainObject getType(SpawnType type) {
        List<TerrainObject> TypeList = new List<TerrainObject>();
        foreach (var item in terrainList) {
            if (item.spawntype == type) {
                TypeList.Add(item);
            }
        }
        int randomValue = Random.Range(0, TypeList.Count);
        TerrainObject ret = TypeList[randomValue];
        return ret;
    }
}
[System.Serializable] public class TerrainObject {
    public GameObject objectPrefab; 
    public SpawnType spawntype;
}
public enum SpawnType { Surface, Regular, Above};