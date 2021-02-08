using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentScript : MonoBehaviour
{
    [SerializeField]
    GameObject fishPrefab;
    [SerializeField]
    GameObject parentFishPrefab;

    [HideInInspector]
    public Transform subFishSpawn;
    [HideInInspector]
    public GameObject bigFish;
    [HideInInspector]
    public GameObject smallFish;

    [Header("Big Fish Settings")]
    [SerializeField]
    Vector3 spawnPointBigFish;
    [SerializeField]
    public Vector3 goalPointBigFish;
    [SerializeField]
    [Range(5.0f, 20.0f)]
    public float maxSpeedB;
    [SerializeField]
    [Range(1.0f, 5.0f)]
    public float rotationSpeedB;

    [Header("Small Fish Settings")]
    [SerializeField]
    Vector3 spawnPointSmallFish;
    [SerializeField]
    public Vector3 goalPointSmallFish;
    [SerializeField]
    [Range(5.0f, 20.0f)]
    public float maxSpeedS;
    [SerializeField]
    [Range(1.0f, 5.0f)]
    public float rotationSpeedS;

    void Start()
    {
        bigFish = Instantiate(parentFishPrefab, spawnPointBigFish, Quaternion.identity) as GameObject;
        subFishSpawn = bigFish.transform.Find("spawnArea");
        smallFish = Instantiate(fishPrefab, spawnPointSmallFish, Quaternion.identity) as GameObject;
        
        bigFish.GetComponent<MoveTogetherScript>().parentMan = this;
    }

}
