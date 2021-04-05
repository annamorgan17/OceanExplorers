using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockScript : MonoBehaviour
{
    [HideInInspector]
    public FlockManScript fishManager;

    float speed;
    bool turning = false;
    GameObject bubble = null;

    void Start()
    {
        speed = Random.Range(1, fishManager.data.maxSpeed);
        Bubbles(fishManager.data.bubblePrefab);
    }

    void Update()
    {
        Bounds b = new Bounds(fishManager.data.setPoint, fishManager.data.swimLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = fishManager.data.setPoint - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), fishManager.data.rotationSpeed * Time.deltaTime);
            speed = Random.Range(1, fishManager.data.maxSpeed);
            
        }
        else
        {
            if (Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }

        }
        if(Random.Range(0, fishManager.data.randomAmount) < 10)
        {
            Bubbles(fishManager.data.bubblePrefab);
        }
        transform.Translate(0, 0, Time.deltaTime * speed);

    }

    private void ApplyRules()
    {
        GameObject[] fishArray;
        fishArray = fishManager.allFish;

        Vector3 center = fishManager.data.setPoint;
        Vector3 avoid = fishManager.data.setPoint;
        Vector3 goalPos = fishManager.goalPos;

        float averageSpeed = 0.1f;
        float distance;
        int groupSize = 0;

        foreach (GameObject fish in fishArray)
        {
            if (fish != this.gameObject)
            {
                distance = Vector3.Distance(fish.transform.position, this.transform.position);
                if (distance <= fishManager.data.fishDistance)
                {
                    center += fish.transform.position;
                    groupSize++;

                    if (distance < 2)
                    {
                        avoid = avoid + (this.transform.position - fish.transform.position);
                    }

                    FlockScript anotherFlock = fish.GetComponent<FlockScript>();
                    averageSpeed += anotherFlock.speed;
                }
            }
        }
        if (groupSize > 0)
        {
            center = center / groupSize + (goalPos - this.transform.position);
            speed = averageSpeed / groupSize;

            Vector3 direction = (center + avoid) - transform.position;
            if (direction != fishManager.data.setPoint)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), fishManager.data.rotationSpeed * Time.deltaTime);

        }

    }

    private void Bubbles(GameObject bubblePrefab)
    {
        bubble = Instantiate(bubblePrefab, this.transform.position, Quaternion.LookRotation(fishManager.mainCamera.transform.position));
        Destroy(bubble, 10f);

    }

}
