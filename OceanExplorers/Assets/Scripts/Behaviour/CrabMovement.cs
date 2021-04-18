using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabMovement : MonoBehaviour {
    public CrabData data;
    public AudioSource sound;
    public AudioClip crabClip;
    private Vector3 newPos = Vector3.zero;
    private RaycastHit hit;

    private void Start()
    {
        if (GetComponent<NavMeshAgent>() == null)
        { // adding a navmesh if needed
            gameObject.AddComponent<NavMeshAgent>();
        }
        TargetPosInstant();
    }
    private void Update()
    {
        if (GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            GetComponent<NavMeshAgent>().updateUpAxis = false;
            GetComponent<NavMeshAgent>().destination = newPos;

            Physics.Raycast(this.transform.position, Vector3.down, out hit);
            transform.up -= (transform.up - hit.normal) * 0.1f;
            sound.PlayOneShot(crabClip, 0.3f);
        }
        TargetPos();

    }


    private void TargetPos() {
        if (Random.Range(0, data.randomAmount) < 50) {
            newPos = new Vector3(Random.Range(-data.gameArea.x, data.gameArea.x),
                                 Random.Range(-data.gameArea.y, data.gameArea.y),
                                 Random.Range(-data.gameArea.z, data.gameArea.z)); 
 
        }
    }

    private void TargetPosInstant() {
        newPos = new Vector3(Random.Range(-data.gameArea.x, data.gameArea.x),
                             Random.Range(-data.gameArea.y, data.gameArea.y),
                             Random.Range(-data.gameArea.z, data.gameArea.z)); 

    }
}
