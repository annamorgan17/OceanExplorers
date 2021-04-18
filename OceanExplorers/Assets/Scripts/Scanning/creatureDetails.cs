using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//stores the data abount fish and adds to total collected
public class creatureDetails : MonoBehaviour
{
    public string[] names = { "Shark", "Crab" };
    public string[] descriptions = {"Big and Scary", "sharp claws"};
    public int totalScanned = 0;
    public GameObject[] creatures;
    
    public void increaseScanned()
    {
        totalScanned++;
    }
}
