using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaTeleport : MonoBehaviour
{
    //String stores the scene to change to (typed exactly as the scene is named)
    public string area;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entering area: " + area);
        //Loads scene
        SceneManager.LoadScene(area);
    }
}
