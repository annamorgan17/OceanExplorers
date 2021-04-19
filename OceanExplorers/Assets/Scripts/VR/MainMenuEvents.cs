using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuEvents : MonoBehaviour
{
    [SerializeField] GameObject Help;
    public AudioSource sound;
    public AudioClip lever;
    public AudioClip button; 

    //the lever button is pressed
    public void StartButtonPressed() {
        Debug.LogError("Start Button was pressed, loading the main scene");
        sound.PlayOneShot(lever, 0.5f);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    //the exit button is pressed
    public void ExitButton() {
        Debug.LogError("Exit the game");
        sound.PlayOneShot(button, 0.5f);
        Application.Quit();
    }
    //show the help screen 
    public void ShowHelp() {
        Debug.LogError("Show the help");
        Help.SetActive(!Help.activeInHierarchy);
        sound.PlayOneShot(button, 0.5f); 
    }
}
