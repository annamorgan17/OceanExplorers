using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//script that controls the movement of the crabs
public class CrabMovement : MonoBehaviour
{
    public CrabData data;//scriptable object
    public AudioSource sound; //aduio source connection
    public AudioClip crabClip; //audio for crab sound effect
    private Vector3 newPos = Vector3.zero; //vector for the new position
    private RaycastHit hit; //new ray hit

    private void Start()
    {
        if (GetComponent<NavMeshAgent>() == null) //if doesnt have nav agent
        { // adding a navmesh if needed
            gameObject.AddComponent<NavMeshAgent>(); //add one
        }
        TargetPosInstant();
    }
    private void Update()
    {
        if (GetComponent<NavMeshAgent>().isOnNavMesh) //if nav agent
        {
            GetComponent<NavMeshAgent>().updateUpAxis = false; //dont update up axis
            GetComponent<NavMeshAgent>().destination = newPos; //set the destination of th crab to its new position

            Physics.Raycast(this.transform.position, Vector3.down, out hit); //draw a ray straight down
            transform.up -= (transform.up - hit.normal) * 0.1f; //rotate the crabs up direction to correlate to the terrain below it
            sound.PlayOneShot(crabClip, 0.3f); //play the crab sound effect
        }
        TargetPos();

    }

    //creates a vector randomly within a set bound, will randomly update to a different location 
    private void TargetPos()
    {
        if (Random.Range(0, data.randomAmount) < 50)
        {
            newPos = new Vector3(Random.Range(-data.gameArea.x, data.gameArea.x),
                                 Random.Range(-data.gameArea.y, data.gameArea.y),
                                 Random.Range(-data.gameArea.z, data.gameArea.z));

        }
    }
    //creates a vector randomly within a set bound
    private void TargetPosInstant()
    {
        newPos = new Vector3(Random.Range(-data.gameArea.x, data.gameArea.x),
                             Random.Range(-data.gameArea.y, data.gameArea.y),
                             Random.Range(-data.gameArea.z, data.gameArea.z));

    }
}
