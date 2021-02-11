using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject canvas, player, secondaryCamera;
    [SerializeField] Button vrOnButton, TeleportOnButton , vrOffButton, TeleportOffButton;

    private bool canvasEnabled = false;
    private bool vrEnabled = true;
    private bool teleportEnabled = false;
    private int vrOnID = 0, vrOffIF = 1, tpOnID = 2, tpOffID = 3;

    private Color grey = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color white = new Color(1f, 1f, 1f, 1f);
    private void Start()
    {
        MenuClose();

        vrOnButton.onClick.AddListener(() => ButtonClicked(0));
        vrOffButton.onClick.AddListener(() => ButtonClicked(1));
        TeleportOnButton.onClick.AddListener(() => ButtonClicked(2));
        TeleportOffButton.onClick.AddListener(() => ButtonClicked(3));
    }
    private void Update()
    {
        if (Input.GetKeyDown("f") && canvasEnabled == false)
        {
            Debug.Log("F Pressed");

            MenuOpen();
            CursorUnlock();
        }

        else if (Input.GetKeyDown("f") && canvasEnabled == true)
        {
            Debug.Log("F Pressed");

            MenuClose();
            CursorLock();
        }
    }

    /*private void ButtonClicked(int buttonID)
    {
        switch (buttonID)
        {
            case 0:
                ActiveButton(vrOnButton);
                UnactiveButton(vrOffButton);
                break;
            case 1:
                ActiveButton(TeleportOnButton);
                UnactiveButton(TeleportOffButton);
                break;
            case 2:
                ActiveButton(vrOffButton);
                UnactiveButton(vrOnButton);
                break;
            case 3:
                ActiveButton(TeleportOffButton);
                UnactiveButton(TeleportOnButton);
                break;

        }

    }*/
    
    private void CursorUnlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void MenuOpen()
    {
        canvas.SetActive(true);
        canvasEnabled = true;
        player.SetActive(false);
        secondaryCamera.SetActive(true);
    }

    private void MenuClose()
    {
        canvas.SetActive(false);
        canvasEnabled = false;
        player.SetActive(true);
        secondaryCamera.SetActive(false);
    }

    private void ActiveButton(Button button)
    {
        Debug.Log(button + " pressed");
        ColorBlock cb = button.colors;
        cb.normalColor = grey;
        button.colors = cb;
    }

    private void UnactiveButton(Button button)
    {
        Debug.Log(button + " pressed");
        ColorBlock cb = button.colors;
        cb.normalColor = white;
        button.colors = cb;
    }
}
