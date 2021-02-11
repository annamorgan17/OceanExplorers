using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject canvas, basicPlayer, vrPlayer, secondaryCamera;
    [SerializeField] Button vrOnButton, TeleportOnButton , vrOffButton, TeleportOffButton;

    private bool canvasEnabled = false;
    private bool vrEnabled = true;
    private bool teleportEnabled = false;
    private int vrOnID = 0, vrOffID = 2, tpOnID = 1, tpOffID = 3;
    private GameObject activePlayer;

    private Color grey = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color white = new Color(1f, 1f, 1f, 1f);
    private void Start()
    {
        MenuClose();

        vrOnButton.onClick.AddListener(() => ButtonClicked(vrOnID));
        vrOffButton.onClick.AddListener(() => ButtonClicked(vrOffID));
        TeleportOnButton.onClick.AddListener(() => ButtonClicked(tpOnID));
        TeleportOffButton.onClick.AddListener(() => ButtonClicked(tpOffID));
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

    private void ButtonClicked(int buttonID)
    {
        switch (buttonID)
        {
            case 0:
                ActiveButton(vrOnButton);
                UnactiveButton(vrOffButton);

                activatePlayerState("vr");
                break;
            case 1:
                ActiveButton(TeleportOnButton);
                UnactiveButton(TeleportOffButton);
                break;
            case 2:
                ActiveButton(vrOffButton);
                UnactiveButton(vrOnButton);

                activatePlayerState("basic");
                break;
            case 3:
                ActiveButton(TeleportOffButton);
                UnactiveButton(TeleportOnButton);
                break;

        }

    }
    
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
        activePlayer.SetActive(false);
        secondaryCamera.SetActive(true);
    }

    private void MenuClose()
    {
        canvas.SetActive(false);
        canvasEnabled = false;
        activePlayer.SetActive(true);
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

    private void activatePlayerState(string playerToActivate)
    {
        if (playerToActivate == "vr")
        {
            basicPlayer.SetActive(false);
            vrPlayer.SetActive(true);

            activePlayer = vrPlayer;
        }
        else if (playerToActivate == "basic")
        {
            basicPlayer.SetActive(true);
            vrPlayer.SetActive(false);

            activePlayer = basicPlayer;
        }
    }
}
