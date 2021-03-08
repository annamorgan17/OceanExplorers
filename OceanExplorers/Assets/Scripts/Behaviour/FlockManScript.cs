using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManScript : MonoBehaviour
{
    public FlockData data;
    public Camera mainCamera;
    [HideInInspector] public GameObject[] allFish = null;
    [HideInInspector] public Vector3 goalPos = Vector3.zero;
    void Start()
    {
        goalPos = data.setPoint;
        allFish = new GameObject[data.fishAmount];
        for (int i = 0; i < data.fishAmount; i++)
        {
            Vector3 position = new Vector3(
                                    Random.Range((data.setPoint.x - data.swimLimits.x), (data.setPoint.x + data.swimLimits.x)),
                                    Random.Range((data.setPoint.y - data.swimLimits.y), (data.setPoint.y + data.swimLimits.y)),
                                    Random.Range((data.setPoint.z - data.swimLimits.z), (data.setPoint.z + data.swimLimits.z)));
            allFish[i] = (GameObject)Instantiate(data.fishprefab, position, Quaternion.identity);
            allFish[i].GetComponent<FlockScript>().fishManager = this;
        }
    }

    void Update()
    {
        data.setPoint = transform.position;
        GoalPosRandom();
    }

    private void GoalPosRandom()
    {
        if (Random.Range(0, data.randomAmount) < 50)
        {
            goalPos = new Vector3(
                                    Random.Range(data.setPoint.x - data.swimLimits.x, data.setPoint.x + data.swimLimits.x),
                                    Random.Range(data.setPoint.y - data.swimLimits.y, data.setPoint.y + data.swimLimits.y),
                                    Random.Range(data.setPoint.z - data.swimLimits.z, data.setPoint.z + data.swimLimits.z));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(data.swimLimits.x * 2, data.swimLimits.y * 2, data.swimLimits.z * 2));
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawSphere(goalPos, 1f);
    }
}
