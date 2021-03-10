using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class factBoxScript : MonoBehaviour
{
    selectingObjScript check;
    [SerializeField]
    TMP_Text objName;
    [SerializeField]
    TMP_Text objDesc;
    [SerializeField]
    TMP_Text totalScanned;
    [SerializeField]
    GameObject factBox;

    private void Start()
    {
        check = this.GetComponent<selectingObjScript>();
    }

    void Update()
    {
        check.CheckObjectClicked();
        if(check.hitOrNot)
        {
            objName.text = check.selectedName;
            objDesc.text = check.selectedDesc;
            totalScanned.text = check.totalScanned.ToString();
            factBox.SetActive(true);
        }
    }
}
