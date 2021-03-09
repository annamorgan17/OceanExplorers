using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageSwitch : MonoBehaviour {
    [SerializeField] Sprite ImageON;
    [SerializeField] Sprite ImageOFF;
    [SerializeField] bool status = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Click() {
        status = !status;
        if (status) {
            gameObject.GetComponent<Image>().sprite = ImageON;
        } else {
            gameObject.GetComponent<Image>().sprite = ImageOFF;
        }
        
    }
}
