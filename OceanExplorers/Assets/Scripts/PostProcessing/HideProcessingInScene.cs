using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class HideProcessingInScene : MonoBehaviour
{
    [SerializeField] PostProcessVolume m_PostProcessVolume = null;
    // Start is called before the first frame update
    void Start()
    {
        m_PostProcessVolume.enabled = true;
    }

    // Update is called once per frame
    void Update()
    { 
    }
}
