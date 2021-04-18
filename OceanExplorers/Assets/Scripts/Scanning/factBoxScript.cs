using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//handles changing the text on the fact box canvas to the required data of each fish
public class factBoxScript : MonoBehaviour
{
    selectingObjScript check; //connection to selecting obj script
    [SerializeField]
    TMP_Text objName; //fish name text field
    [SerializeField]
    TMP_Text objDesc; //fish description text field
    [SerializeField]
    TMP_Text totalScanned; //total scanned fish text field
    [SerializeField]
    GameObject factBox; //entire gameobject of the fact box

    private void Start()
    {
        check = this.GetComponent<selectingObjScript>(); //connect scirpt
    }

    void Update()
    {
        if(check.hitOrNot) //if the fish is selected set the rext fields to the needed data then set the fact box to active
        {
            objName.text = check.selectedName;
            objDesc.text = check.selectedDesc;
            totalScanned.text = check.totalScanned.ToString();
            factBox.SetActive(true);
        }
    }
}
