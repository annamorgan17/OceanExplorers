using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ToggleButton : MonoBehaviour {
    [SerializeField] Image Background;
    [SerializeField] Image Handle;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color OffColour;
    bool status = false;
    Vector3[] positions = new Vector3[2];
    // Start is called before the first frame update
    void Start()
    {
        Vector3[] v = new Vector3[4];
        Vector3[] a = new Vector3[4];
        Background.GetComponent<RectTransform>().GetWorldCorners(v);
        Background.GetComponent<RectTransform>().GetWorldCorners(a);
        positions[0] = new Vector3(v[0].x + (Handle.GetComponent<RectTransform>().sizeDelta.x / 2), Handle.GetComponent<RectTransform>().transform.position.y, 0);
        positions[1] = new Vector3(v[3].x - (Handle.GetComponent<RectTransform>().sizeDelta.x / 2), Handle.GetComponent<RectTransform>().transform.position.y, 0);

        Click();
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Click() {
        status = !status;
        if (status) {
            Handle.GetComponent<RectTransform>().transform.position = positions[1];
            Background.color = Color.white;
            text.text = "ON";
        } else {
            Handle.GetComponent<RectTransform>().transform.position = positions[0]; 
            Background.color = OffColour;
            text.text = "OFF";
        }
    }
}
