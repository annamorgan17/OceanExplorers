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
    [HideInInspector]
    public Vector3 setPoint = Vector3.zero;

    [Header("School Settings")]
    public Vector3 swimLimits = Vector3.zero;
    [SerializeField]
    int randomAmount = 10000;
    [Range(10.0f, 30.0f)]
    public float maxSpeed;
    [Range(0.5f, 10.0f)]
    public float fishDistance;
    [Range(5f, 10f)]
    public float rotationSpeed;
    [Range(2.0f, 500.0f)]
    public int fishAmount = 10;

    void Start()
    {
        goalPos = setPoint;
        allFish = new GameObject[fishAmount];
        for (int i = 0; i < fishAmount; i++)
        {
            Vector3 position = new Vector3(Random.Range((setPoint.x - swimLimits.x), (setPoint.x + swimLimits.x)),
                                   Random.Range((setPoint.y - swimLimits.y), (setPoint.y + swimLimits.y)),
                                   Random.Range((setPoint.z - swimLimits.z), (setPoint.z + swimLimits.z)));
            allFish[i] = (GameObject)Instantiate(fishprefab, position, Quaternion.identity);
            allFish[i].GetComponent<FlockScript>().fishManager = this;
        }
    }

    void Update()
    {
        setPoint = transform.position;
        GoalPosRandom();
    }

    private void GoalPosRandom()
    {
        if (Random.Range(0, 10000) < 50)
        {
            goalPos = new Vector3(Random.Range(setPoint.x - swimLimits.x, setPoint.x + swimLimits.x),
                                   Random.Range(setPoint.y - swimLimits.y, setPoint.y + swimLimits.y),
                                   Random.Range(setPoint.z - swimLimits.z, setPoint.z + swimLimits.z));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(swimLimits.x * 2, swimLimits.y * 2, swimLimits.z * 2));
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawSphere(goalPos, 1f);
    }
}
