using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarManager : MonoBehaviour
{

    // Private Cached References
    private CanvasGroup toolbarCanvasGroup;

    // Private variables
    private bool isToolbarOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
    }

    private void SetReferences()
    {
        toolbarCanvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!isToolbarOpen)
                ShowToolbar();
            else if (isToolbarOpen)
                HideToolbar();
        }

        /* NOTE:    In the Unity editor, the close toolbar function does not work as intended. 
         *          On pressing escape or T while toolbar is open to close the toolbar, the mouse cursor does not disappear.
         *          However, in the game build, this function works as expected when pressing escape to close the toolbar.
        */

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isToolbarOpen)
            {
                HideToolbar();
            }
        }
    }

    public void ShowToolbar()
    {
        isToolbarOpen = true;

        // Show toolbar display
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(toolbarCanvasGroup, 0f, 1f, 0f, 0.3f));
        // Enable mouse interaction
        toolbarCanvasGroup.interactable = true;
        toolbarCanvasGroup.blocksRaycasts = true;

        // Unlock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;

        // Stop player movement
        if (Player.instance)
        {
            Player.StopMovement();
        }
        else if (PlayerTopDown.instance)
        {
            PlayerTopDown.StopMovement();
        }

        // Disable opening pause menu on pressing escape key
        // GameSession.instance.CanPauseGame = false;
        PauseMenu.instance.CanPauseGame = false;
    }

    public void HideToolbar()
    {
        isToolbarOpen = false;

        // Hide Toolbar display
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(toolbarCanvasGroup, 1f, 0f, 0f, 0.3f));
        // Prevent mouse interaction
        toolbarCanvasGroup.interactable = false;
        toolbarCanvasGroup.blocksRaycasts = false;

        // Lock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Resume player movement
        if (Player.instance)
        {
            Player.EnableMovement();
        }
        else if (PlayerTopDown.instance)
        {
            PlayerTopDown.EnableMovement();
        }

        // Enable opening pause menu on pressing escape key
        // StartCoroutine(GameSession.EnableCanPauseGameBoolAfterDelay(0.1f));
        StartCoroutine(PauseMenu.EnableCanPauseGameBoolAfterDelay(0.1f));
    }
}
