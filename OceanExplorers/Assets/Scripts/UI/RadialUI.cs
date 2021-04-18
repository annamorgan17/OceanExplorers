using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RadialUI : MonoBehaviour
{
    // Public UI References
    public Image fillImage;
    public GameObject handlePivot;
    public TextMeshProUGUI displayText; 
    public Button button;
    // Trackers for min/max values
    protected float maxValue = 360f, minValue = 0f;

    public AudioSource AS;
    // Create a property to handle the slider's value
    private float currentValue = 0f;
    public float CurrentValue {
        get {
            return currentValue;
        }
        set {
            // Ensure the passed value falls within min/max range
            currentValue = Mathf.Clamp(value, minValue, maxValue);

            // Calculate the current fill percentage and display it
            float fillPercentage = currentValue / maxValue; 
            fillImage.fillAmount = fillPercentage;  
            displayText.text = (fillPercentage * 100).ToString("0") + "%"; 
        }
    }
    float pressValue = 0;
    bool Pressed = false;
    float StartX = 0; 
    void Start() { 
        CurrentValue = 0f; 
    }
    public void OnClick() { 
        pressValue = currentValue;
        Pressed = !Pressed;
        StartX = Input.mousePosition.x;
    } 
    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            
            if (Pressed) { 
                Pressed = false;
            }
            
        }
        if (Pressed == true) { 
            CurrentValue = pressValue + ( (StartX -  Input.mousePosition.x) / 1);
            RectTransform rectTransform = handlePivot.GetComponent<RectTransform>();
            rectTransform.localEulerAngles = new Vector3(0, 0, 360 - (CurrentValue));
            AS.volume = currentValue / 360;
            Debug.LogError(currentValue / 360);
        }
    }
}
