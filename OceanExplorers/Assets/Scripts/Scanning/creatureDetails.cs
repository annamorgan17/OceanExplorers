using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creatureDetails : MonoBehaviour
{
    public string[] names = { "GoldFish", "Crab" };
    public string[] descriptions = {"gold and small", "sharp claws"};
    public int totalScanned = 0;
    public GameObject[] creatures;
    
    public void increaseScanned()
    {
        totalScanned++;
    }
}
