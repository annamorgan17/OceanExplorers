using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//NO LONGER IN USE
//previously made a toggle switch
public class ImageSwitch : MonoBehaviour {
    [SerializeField] Sprite ImageON;
    [SerializeField] Sprite ImageOFF;
    [SerializeField] bool status = false; 
    public void Click() {
        status = !status;
        if (status) {
            gameObject.GetComponent<Image>().sprite = ImageON;
        } else {
            gameObject.GetComponent<Image>().sprite = ImageOFF;
        }
        
    }
}
