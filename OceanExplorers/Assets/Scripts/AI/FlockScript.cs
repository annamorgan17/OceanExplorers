using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockScript : MonoBehaviour
{
    [HideInInspector]
    public FlockManScript fishManager; //connection to flock manager

    float speed; //speed of individual fish
    bool turning = false; //is the fish turning
    //bool floor = false;
    GameObject bubble = null; //game object for bubble partical

    void Start()
    {
        speed = Random.Range(1, fishManager.data.maxSpeed); //sets speed to random number from 0 to the set max speed
        //Bubbles(fishManager.data.bubblePrefab);
        // bubble.transform.parent = this.transform; //creates the fish as the bubble parent
        fishManager.sound.PlayOneShot(fishManager.bubbleClip, 0.5f); //plays the bubble sound effect
    }

    void Update()
    {

        Bounds b = new Bounds(fishManager.data.setPoint, fishManager.data.swimLimits * 2); //creates a new bound the size of the swim limit

        //foreach (GameObject t in fishManager.terrain)
        //{
        //    float distance = Vector3.Distance(transform.position, t.transform.position);
        //    if (distance <= 1)
        //    {
        //        floor = true;
        //    }
        //    else
        //    {
        //        floor = false;
        //    }
        //}
        //if(floor == true)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Inverse(transform.rotation), fishManager.data.rotationSpeed * Time.deltaTime);
        //}

        if (!b.Contains(transform.position)) //if not in bound
        {
            turning = true;
        }
        else //if in bounds
        {
            turning = false;
        }

        if (turning) //if out of bound turn the fish towards the centre at a new speed 
        {
            Vector3 direction = fishManager.data.setPoint - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), fishManager.data.rotationSpeed * Time.deltaTime);
            fishManager.sound.PlayOneShot(fishManager.swishClip, 0.5f); //plays the fish swimming sound effect
            speed = Random.Range(1, fishManager.data.maxSpeed);

        }
        else //if in bounds randomly apply the boids rules
        {
            if (Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }

        }
        if (Random.Range(0, fishManager.data.randomAmount) < 10000) // randomly create bubble and play sound effect
        {
            Bubbles(fishManager.data.bubblePrefab);
            // bubble.transform.parent = this.transform; //creates the fish as the bubble parent
            fishManager.sound.PlayOneShot(fishManager.bubbleClip, 0.5f);
        }
        transform.Translate(0, 0, Time.deltaTime * speed); //move the fish at its speed

    }

    private void ApplyRules()
    {
        GameObject[] fishArray; //array of all the fish within that prefab
        fishArray = fishManager.allFish;

        Vector3 center = fishManager.data.setPoint; //set point acting as 0
        Vector3 avoid = fishManager.data.setPoint;
        Vector3 goalPos = fishManager.goalPos;
        //variables for the group
        float averageSpeed = 0.1f;
        float distance;
        int groupSize = 0;

        foreach (GameObject fish in fishArray) //loop through all the fish
        {
            if (fish != this.gameObject) //if the current fish isnt the same as the fish connected to  this script right now
            {
                distance = Vector3.Distance(fish.transform.position, this.transform.position); //calc the distance
                if (distance <= fishManager.data.fishDistance) //if under the set distance for neighbour fish
                {
                    center += fish.transform.position; //adjust the flock centre
                    groupSize++; //increase the flock size

                    if (distance < 2) //if distance is even small
                    {
                        avoid = avoid + (this.transform.position - fish.transform.position); //change the avoid to avoid that fish
                    }

                    FlockScript anotherFlock = fish.GetComponent<FlockScript>(); //get the script on that fish
                    averageSpeed += anotherFlock.speed; //increase group speed by other fishes speed
                }
            }
        }
        if (groupSize > 0) //if there is a group
        {
            center = center / groupSize + (goalPos - this.transform.position); //adjust the group centre to include the group size current pos and goal pos
            speed = averageSpeed / groupSize; //adjust this fishes speed by the group and group speed

            Vector3 direction = (center + avoid) - transform.position; //adjust the direction to include the centre and avoid of the group
            if (direction != fishManager.data.setPoint) //if the direction isnt the set point
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), fishManager.data.rotationSpeed * Time.deltaTime); //rotate the fish towards the new direction

        }

    }
    //creates an instance of bubbles for 10f
    private void Bubbles(GameObject bubblePrefab)
    {
        bubble = Instantiate(bubblePrefab, this.transform.position, Quaternion.LookRotation(Camera.main.transform.position));
        Destroy(bubble, 2f);

    }
}
