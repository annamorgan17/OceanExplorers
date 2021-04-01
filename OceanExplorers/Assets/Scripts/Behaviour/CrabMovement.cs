using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabMovement : MonoBehaviour {
    public CrabData data;
    private Vector3 newPos = Vector3.zero;

    private void Start() {
        if (GetComponent<NavMeshAgent>() == null) { // adding a navmesh if needed
            gameObject.AddComponent<NavMeshAgent>();
        } 
        TargetPosInstant();
    }
    private void Update() {
        if (GetComponent<NavMeshAgent>().isOnNavMesh) {
            GetComponent<NavMeshAgent>().destination = newPos;
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
