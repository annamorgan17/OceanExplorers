using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManScriptPlaty : MonoBehaviour
{
    public FlockData data; //scriptable object
    [HideInInspector] public GameObject[] allFish = null; // array for all the fish as game objects
    public Vector3 setPoint = Vector3.zero;
    //public GameObject[] terrain = null;
    [HideInInspector] public Vector3 goalPos = Vector3.zero;// the goal position the fish will head towards
    public AudioSource sound; //audio source connection
    public AudioClip bubbleClip; //sound effect of bubbles
    public AudioClip swishClip; //sound effect of fish swimming
    private int counter = 0; //counter to help know how many times a loop has occured
    void Start()
    {
        goalPos = setPoint; //set the goal point to the set point
                                 //foreach (GameObject fish in data.fishprefab) { //for every prefab in the array

        allFish = new GameObject[data.fishAmount[3]]; //set array size to the amount of fish of that prefab
        for (int i = 0; i < data.fishAmount[3]; i++)
        { //loop through all those fish
            if (i >= 0 & i < allFish.Length)
            { // overiding to stop the headset lagging out
              // if (data.fishprefab[i] != null) { // overiding to stop the headset lagging out
                Vector3 position = new Vector3( //create a new vector at a random point within bounds
                                        Random.Range((- data.swimLimits.x), ( data.swimLimits.x)),
                                        Random.Range((- data.swimLimits.y), ( data.swimLimits.y)),
                                        Random.Range((- data.swimLimits.z), ( data.swimLimits.z)));
                allFish[i] = (GameObject)Instantiate(data.fishprefab[3], position, Quaternion.identity); //create an instance of that prefab
                allFish[i].GetComponent<FlockScriptPlaty>().fishManager = this; //link the flock script to this script
                                                                           // }
            }
        }
        //counter++; //increase counter
        //} 

    }


    void Update()
    {
        //setting the set point to the position of manager
        setPoint = transform.position;
        GoalPosRandom();
    }
    //at random intervaules will create a random vector within bounds and set the goal pos to it
    private void GoalPosRandom()
    {
        if (Random.Range(0, data.randomAmount) < 50)
        {
            goalPos = new Vector3(
                                    Random.Range(- data.swimLimits.x,  data.swimLimits.x),
                                    Random.Range(- data.swimLimits.y,  data.swimLimits.y),
                                    Random.Range(- data.swimLimits.z,  data.swimLimits.z));
        }
    }
    //draws a cube around bounds and a sphere at goal pos
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(data.swimLimits.x * 2, data.swimLimits.y * 2, data.swimLimits.z * 2));
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawSphere(goalPos, 1f);
    }
}
