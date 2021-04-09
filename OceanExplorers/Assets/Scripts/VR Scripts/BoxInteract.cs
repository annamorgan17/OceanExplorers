//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BoxInteract : MonoBehaviour
//{
//    //Home Orb gameobject
//    public GameObject homeOrb;
//    RaycastHit hit;
//    private void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.E))
//        {
//            Debug.Log("E press");
//            Debug.Log(gameObject.name);

//            //shoot raycaster certain distance
//            //if hit get gameobject name
//            //if hit right gameobject do trigger
//        }
//    }
//    //Temporary solution to get back, delete once raycaster works
//    private void OnTriggerEnter(Collider other)
//    {
//        Debug.Log("Triggered");
//        InstantiateOrb();
//    }

//    private void InstantiateOrb()
//    {
//        //Instantiate Home Orb at 0, 1, 0
//        Instantiate(homeOrb, new Vector3(0, 1, 0), Quaternion.identity);
//    }
//}
