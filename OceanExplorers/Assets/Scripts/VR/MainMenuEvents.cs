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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartButtonPressed() {
        Debug.LogError("Start Button was pressed, loading the main scene");
        sound.PlayOneShot(lever, 0.5f);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    public void ExitButton() {
        Debug.LogError("Exit the game");
        sound.PlayOneShot(button, 0.5f);
        Application.Quit();
    }
    public void ShowHelp() {
        Help.SetActive(!Help.activeInHierarchy);
        sound.PlayOneShot(button, 0.5f);
        Debug.LogError("Show the help");
    }
}
