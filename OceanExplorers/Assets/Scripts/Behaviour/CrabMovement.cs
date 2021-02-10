using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMovement : MonoBehaviour
{
    [SerializeField]
    Vector3 gameArea = Vector3.zero;
    [SerializeField]
    [Range(0.1f, 5.0f)]
    float crabSpeed;
    [SerializeField]
    [Range(1.0f, 5.0f)]
    float crabRotSpeed;
    Vector3 newPos = Vector3.zero;
    Quaternion targetRotation;
    [SerializeField]
    float randomAmount = 10000;

    Vector3 direction;
    int layerMask = 1 << 8;

    private void Start()
    {
        targetRotation = Quaternion.LookRotation(newPos - transform.position);
        TargetPosInstant();
    }
    private void Update()
    {
        direction = (newPos - transform.position).normalized;


        if (!Physics.Raycast(this.transform.position, direction, 3, layerMask))
        {
            Debug.DrawRay(transform.position, direction * 3, Color.red);

            transform.position = Vector3.MoveTowards(this.transform.position, newPos, crabSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction), crabRotSpeed * Time.deltaTime);

            TargetPos();
        }
        else
        {
            transform.position = Vector3.MoveTowards(this.transform.position, direction * 1, crabSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction * 1), crabRotSpeed * Time.deltaTime);
        }

    }

    private void TargetPos()
    {
        if (Random.Range(0, randomAmount) < 50)
        {
            newPos = new Vector3(Random.Range(-gameArea.x, gameArea.x),
                0,
                Random.Range(-gameArea.z, gameArea.z));

            targetRotation = Quaternion.LookRotation(newPos - transform.position);

        }
    }

    private void TargetPosInstant()
    {
        newPos = new Vector3(Random.Range(-gameArea.x, gameArea.x),
                  0,
                  Random.Range(-gameArea.z, gameArea.z));

        targetRotation = Quaternion.LookRotation(newPos - transform.position);


    }
}
