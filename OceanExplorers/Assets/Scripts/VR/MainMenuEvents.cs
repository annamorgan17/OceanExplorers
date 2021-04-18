using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuEvents : MonoBehaviour
{
    [SerializeField] GameObject Help;
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
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);
    }
    public void ExitButton() {
        Debug.LogError("Exit the game");
        Application.Quit();
    }
    public void ShowHelp() {
        Help.SetActive(!Help.activeInHierarchy);
        Debug.LogError("Show the help");
    }
}
