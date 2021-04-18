using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition; 

//Option menu functiosn
public class OptionMenuFunctions : MonoBehaviour {
    //reference to light and volume to adjust
    [SerializeField] Light light;
    [SerializeField] Volume volume;

    //change the chunk distance
    public void ChunkDistance(float percentage) { 
        float Default1 = 50, Default2 = 100, Default3 = 200;

        //multiple the percentage by the default values
        Default1 *= percentage; Default2 *= percentage; Default3 *= percentage;

        //set the distances
        TerrainGenerator tg = Object.FindObjectOfType<TerrainGenerator>();
        tg.detailLevels[0].visibleDstThreshold = Default1;
        tg.detailLevels[1].visibleDstThreshold = Default2;
        tg.detailLevels[2].visibleDstThreshold = Default3;

        Debug.Log("Changed the view distance to: " + Default1 + " / " + Default2 + " / " + Default3);
    }
    //set the fog strength
    public void fogStrength(float percentage) { 
        Fog fog;
        volume.profile.TryGet(out fog);
        fog.baseHeight.value = percentage;
        Debug.LogError("Fog is at: " + (percentage)); 
    }
    //change the directional light intensity
    public void Brightness(float percentage) {
        light.intensity = percentage;
    }

    //unused, would change the scale of the sand texture
    /*
    public void textureScale(float percentage) {
        foreach (var item in Object.FindObjectsOfType<Transform>()) {
            if (item.GetComponent<MeshRenderer>()) {
                if (item.GetComponent<MeshRenderer>().material.name == "sand") {
                    Debug.LogError("SandFound");
                    Material sand = item.GetComponent<MeshRenderer>().material; 
                    float max = 10;
                    sand.SetInt("_UVScale", (int)(max / percentage));
                }
            }
            
        }
    } */

    //load the MainMenu scene
    public void MainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    //exit the game
    public void ExitGame() {
        Application.Quit();
    }

    //change the music volume
    //currently unused, replaced in radial script
    public void MusicVolume(float percentage) {
        float max = 10;
        AudioListener.volume = max / percentage;
    }
}
