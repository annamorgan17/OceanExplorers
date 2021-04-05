using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class movement : MonoBehaviour
{
    public Vector3 gameArea;
    public int randomAmount;
    private Vector3 newPos = Vector3.zero;
    private RaycastHit hit;
    public Transform raycastPoint;

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
            GetComponent<NavMeshAgent>().destination = newPos;
            if (Physics.Raycast(transform.position, -transform.up, out hit))
            {
                var slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
            }
            GetComponent<NavMeshAgent>().updateUpAxis = false;
            
        }
        TargetPos();

    }

    private void TargetPos()
    {
        if (Random.Range(0, randomAmount) < 50)
        {
            newPos = new Vector3(Random.Range(-gameArea.x, gameArea.x),
                                 Random.Range(-gameArea.y, gameArea.y),
                                 Random.Range(-gameArea.z, gameArea.z));

        }
    }

    private void TargetPosInstant()
    {
        newPos = new Vector3(Random.Range(-gameArea.x, gameArea.x),
                             Random.Range(-gameArea.y, gameArea.y),
                             Random.Range(-gameArea.z, gameArea.z));

    }



}
