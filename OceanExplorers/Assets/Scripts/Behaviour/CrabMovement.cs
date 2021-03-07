using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabMovement : MonoBehaviour {
    public CrabData data;
    private Vector3 newPos = Vector3.zero;
    private Quaternion targetRotation;
    private Vector3 direction; 

    private void Start() {
        if (GetComponent<NavMeshAgent>() == null) { // adding a navmesh if needed
            gameObject.AddComponent<NavMeshAgent>();
        } 
        targetRotation = Quaternion.LookRotation(newPos - transform.position);
        TargetPosInstant(); 
    }
    private void Update() {
        direction = (newPos - transform.position).normalized;
        Debug.DrawRay(transform.position, direction * data.rayLength, Color.red);
        GetComponent<NavMeshAgent>().destination = newPos;
        TargetPos();

        //if (!Physics.Raycast(this.transform.position, direction, rayLength, layerMask)) {
        //    Debug.DrawRay(transform.position, direction * rayLength, Color.red); 
        //    transform.position = Vector3.MoveTowards(this.transform.position, newPos, crabSpeed * Time.deltaTime);
        //    transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction), crabRotSpeed * Time.deltaTime);
        //    TargetPos();
        //} else { 
        //    transform.position = Vector3.MoveTowards(this.transform.position, direction * 1, crabSpeed * Time.deltaTime);
        //    transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction * 1), crabRotSpeed * Time.deltaTime);
        //} 
    }

    private void TargetPos() {
        if (Random.Range(0, data.randomAmount) < 50) {
            newPos = new Vector3(Random.Range(-data.gameArea.x, data.gameArea.x),
                                 gameObject.transform.position.y,
                                 Random.Range(-data.gameArea.z, data.gameArea.z)); 
            targetRotation = Quaternion.LookRotation(newPos - transform.position); 
        }
    }

    private void TargetPosInstant() {
        newPos = new Vector3(Random.Range(-data.gameArea.x, data.gameArea.x),
                             gameObject.transform.position.y,
                             Random.Range(-data.gameArea.z, data.gameArea.z)); 
        targetRotation = Quaternion.LookRotation(newPos - transform.position); 
    }
}
