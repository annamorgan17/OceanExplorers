using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//creates a ray after input to select a fish then checks the details connected to that fish
public class selectingObjScript : MonoBehaviour
{
    [HideInInspector]
    public string selectedDesc;
    [HideInInspector]
    public string selectedName;
    [HideInInspector]
    public int totalScanned;
    [HideInInspector]
    public bool hitOrNot = false;

    creatureDetails creature; //connect to creature detail script
    private int counter = 0;

    private void Start()
    {
        creature = this.GetComponent<creatureDetails>(); //connects scripts
    }
    public void CheckObjectClicked()
    {

            Ray ray = Camera.main.ScreenPointToRay((new Vector3(Screen.width / 2, Screen.height / 2, 0))); //creates a ray from the centre of the screen
            RaycastHit hit;
            counter = 0;
            if (Physics.Raycast(ray, out hit)) //if there was a hit
            {
                foreach(GameObject c in creature.creatures) //loop through all the creatures
                {
                    if (hit.transform.tag == c.transform.tag) //checks their tag
                    {
                        hitOrNot = true; //triggers that it was a correct hit
                        //sets the creature deatils to the selected details and increases the total scanned
                        selectedDesc = creature.descriptions[counter]; 
                        selectedName = creature.names[counter];
                        creature.increaseScanned();
                        totalScanned = creature.totalScanned;
                    }
                    counter++; //increases counter
                }
            }
            else //else there wasnt a correct hit
            {
                hitOrNot = false;
            }
        
    }
}
