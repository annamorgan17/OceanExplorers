using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
    [SerializeField] Transform player; 
    // Update is called once per frame
    void Update() {
        //allow the water object to follow the player on the x and z axis
        gameObject.transform.position = new Vector3(player.transform.position.x, gameObject.transform.position.y, player.transform.position.z);
    }
}
