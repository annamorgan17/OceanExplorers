using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManScript : MonoBehaviour
{
    [SerializeField]
    GameObject fishprefab = null;
    [HideInInspector]
    public GameObject[] allFish = null;
    [HideInInspector]
    public Vector3 goalPos = Vector3.zero;

    [Header("School Settings")]
    public Vector3 setPoint = Vector3.zero;
    public Vector3 swimLimits = Vector3.zero;
    [Space(20)]
    [Range(1.0f, 10.0f)]
    public float maxSpeed;
    [Range(0.5f, 5.0f)]
    public float fishDistance;
    [Range(0.5f, 5.0f)]
    public float rotationSpeed;
    [Range(2.0f, 500.0f)]
    public int fishAmount = 10;

    void Start()
    {
        Vector3 startArea = transform.position;
        allFish = new GameObject[fishAmount];
        this.GetComponent<BoxCollider>().size = swimLimits;

        for (int i = 0; i < fishAmount; i++)
        {
            Vector3 position = new Vector3(Random.Range((startArea.x - swimLimits.x), (startArea.x + swimLimits.x)),
                                   Random.Range((startArea.y - swimLimits.y), (startArea.y + swimLimits.y)),
                                   Random.Range((startArea.z - swimLimits.z), (startArea.z + swimLimits.z)));
            allFish[i] = (GameObject)Instantiate(fishprefab, position, Quaternion.identity);
            allFish[i].GetComponent<FlockScript>().fishManager = this;
        }
    }

    void Update()
    {
        GoalPosRandom();
    }

    private void GoalPosRandom()
    {
        if (Random.Range(0, 10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                  Random.Range(-swimLimits.y, swimLimits.y),
                                  Random.Range(-swimLimits.z, swimLimits.z));
        }
    }
}
