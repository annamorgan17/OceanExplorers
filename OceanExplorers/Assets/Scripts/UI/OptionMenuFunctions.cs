using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition; 
public class OptionMenuFunctions : MonoBehaviour {
    [SerializeField] Light light;
    [SerializeField] Volume volume;
    public void ChunkDistance(float percentage) {
        float localPercentage = percentage - 50; // Going from -.5 to .5 to reduce and increase distance
        float Default1 = 50, Default2 = 100, Default3 = 200;
        Default1 *= percentage; Default2 *= percentage; Default3 *= percentage;

        TerrainGenerator tg = Object.FindObjectOfType<TerrainGenerator>();
        tg.detailLevels[0].visibleDstThreshold = Default1;
        tg.detailLevels[1].visibleDstThreshold = Default2;
        tg.detailLevels[2].visibleDstThreshold = Default3;

        Debug.Log("Changed the view distance to: " + Default1 + " / " + Default2 + " / " + Default3);
    }
    public void fogStrength(float percentage) { 
        Fog fog;
        volume.profile.TryGet(out fog);
        fog.baseHeight.value = percentage;
        Debug.LogError("Fog is at: " + (percentage)); 
    }
    public void Brightness(float percentage) {
        light.intensity = percentage;
    }
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
    public void MainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
    public void ExitGame() {
        Application.Quit();
    }
    public void MusicVolume(float percentage) {
        float max = 10;
        AudioListener.volume = max / percentage;
    }
}
