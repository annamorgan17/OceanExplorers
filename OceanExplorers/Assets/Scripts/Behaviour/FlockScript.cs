using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockScript : MonoBehaviour
{
    [HideInInspector]
    public FlockManScript fishManager;

    float speed;
    bool turning = false;

    void Start()
    {
        speed = Random.Range(1, fishManager.maxSpeed);
    }

    void Update()
    {
        Bounds b = new Bounds(fishManager.setPoint, fishManager.swimLimits * 2);

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
            Vector3 direction = fishManager.setPoint - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), fishManager.rotationSpeed * Time.deltaTime);
            speed = Random.Range(1, fishManager.maxSpeed);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }

        }
        transform.Translate(0, 0, Time.deltaTime * speed);

    }

    private void ApplyRules()
    {
        GameObject[] fishArray;
        fishArray = fishManager.allFish;

        Vector3 center = fishManager.setPoint;
        Vector3 avoid = fishManager.setPoint;
        Vector3 goalPos = fishManager.goalPos;

        float averageSpeed = 0.1f;
        float distance;
        int groupSize = 0;

        foreach (GameObject fish in fishArray)
        {
            if (fish != this.gameObject)
            {
                distance = Vector3.Distance(fish.transform.position, this.transform.position);
                if (distance <= fishManager.fishDistance)
                {
                    center += fish.transform.position;
                    groupSize++;

                    if (distance < 1)
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
            if (direction != fishManager.setPoint)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), fishManager.rotationSpeed * Time.deltaTime);

        }

    }

}
