using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour { 
	void Start() {
		//gameobject is false to start with, then is set to active at start 
		gameObject.SetActive(false);
	} 
}
