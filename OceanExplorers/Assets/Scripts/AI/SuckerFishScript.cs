using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//no longer in use
public class SuckerFishScript : MonoBehaviour
{
    [HideInInspector]
    public SuckerFishManScript parentMan;
    float speedB;
    float speedS;
    void Start()
    {
        speedB = Random.Range(5, parentMan.maxSpeedB);
        speedS = Random.Range(5, parentMan.maxSpeedS);
    }

    void Update()
    {
        if (Vector3.Distance(parentMan.subFishSpawn.position, parentMan.smallFish.transform.position) <= 2.0f)
        {
            parentMan.smallFish.transform.position = parentMan.subFishSpawn.position;
            parentMan.smallFish.transform.rotation = parentMan.bigFish.transform.rotation;
            parentMan.smallFish.transform.parent = parentMan.bigFish.transform;

        }
        Vector3 directionB = parentMan.goalPointBigFish - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionB), parentMan.rotationSpeedB * Time.deltaTime);
        transform.Translate(0, 0, Time.deltaTime * speedB);

        Vector3 directionS = parentMan.goalPointSmallFish - parentMan.smallFish.transform.position;
        parentMan.smallFish.transform.rotation = Quaternion.Slerp(parentMan.smallFish.transform.rotation, Quaternion.LookRotation(directionS), parentMan.rotationSpeedS * Time.deltaTime);
        parentMan.smallFish.transform.Translate(0, 0, Time.deltaTime * speedS);
    }
}
