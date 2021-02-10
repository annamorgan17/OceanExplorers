using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishManScript : MonoBehaviour
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
    [Space(20)]
    [Range(1.0f, 10.0f)]
    public float maxSpeed;
    [Range(0.5f, 5.0f)]
    public float fishDistance;
    [Range(0.5f, 5.0f)]
    public float rotationSpeed;
    [Range(2.0f, 500.0f)]
    public int fishAmount = 10;
    [SerializeField]
    int randomAmount = 10000;

    void Start()
    {
        allFish = new GameObject[fishAmount];
        goalPos = setPoint;
        for (int i = 0; i < fishAmount; i++)
        {
            Vector3 position = new Vector3(Random.Range((setPoint.x - swimLimits.x), (setPoint.x + swimLimits.x)),
                                   Random.Range((setPoint.y - swimLimits.y), (setPoint.y + swimLimits.y)),
                                   Random.Range((setPoint.z - swimLimits.z), (setPoint.z + swimLimits.z)));
            allFish[i] = (GameObject)Instantiate(fishprefab, position, Quaternion.identity);
            allFish[i].GetComponent<flockScript>().fishManager = this;
        }
    }

    void Update()
    {
        setPoint = transform.position;
        GoalPosRandom();
    }

    private void GoalPosRandom()
    {
        if (Random.Range(0, randomAmount) < 50)
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
