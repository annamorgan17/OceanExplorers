using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    creatureDetails creature;
    private int counter = 0;

    private void Start()
    {
        creature = this.GetComponent<creatureDetails>();
    }
    public void CheckObjectClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            counter = 0;
            if (Physics.Raycast(ray, out hit))
            {
                foreach(GameObject c in creature.creatures)
                {
                    if (hit.transform.tag == c.transform.tag)
                    {
                        hitOrNot = true;
                        selectedDesc = creature.descriptions[counter];
                        selectedName = creature.names[counter];
                        creature.increaseScanned();
                        totalScanned = creature.totalScanned;
                    }
                    counter++;
                }
            }
            else
            {
                hitOrNot = false;
            }
        }
    }
}
