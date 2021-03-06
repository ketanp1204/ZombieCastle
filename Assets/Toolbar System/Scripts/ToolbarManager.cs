﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarManager : MonoBehaviour
{
    // Singleton
    public static ToolbarManager instance;

    // Public references
    public Image inventoryBagHighlight;

    // Private references
    private CanvasGroup toolbarCanvasGroup;

    // Private variables
    private bool canOpenToolbar;
    private bool isToolbarOpen;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        toolbarCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Initialize()
    {
        if (GameData.lobby_introDialogueSeen)
            canOpenToolbar = true;
        else
            canOpenToolbar = false;

        isToolbarOpen = false;

        toolbarCanvasGroup.alpha = 0f;
        toolbarCanvasGroup.interactable = false;
        toolbarCanvasGroup.blocksRaycasts = false;
    }

    public static void DisableToolbarOpen()
    {
        instance.canOpenToolbar = false;
    }

    public static void EnableToolbarOpen()
    {
        instance.canOpenToolbar = true;
    }

    // Update is called once per frame
    void Update()
    {
        /* NOTE:    In the Unity editor, the close toolbar function does not work as intended. 
         *          On pressing escape or T while toolbar is open to close the toolbar, the mouse cursor does not disappear.
         *          However, in the game build, this function works as expected when pressing escape to close the toolbar.
        */

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!isToolbarOpen)
                ShowToolbar();
            else if (isToolbarOpen)
                HideToolbar();
        }
    }

    public void ShowToolbar()
    {
        if (canOpenToolbar)
        {
            isToolbarOpen = true;

            // Show toolbar display
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(toolbarCanvasGroup, 0f, 1f, 0f, 0.3f));
            toolbarCanvasGroup.interactable = true;
            toolbarCanvasGroup.blocksRaycasts = true;

            // Unlock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void HideToolbar()
    {
        if (toolbarCanvasGroup.alpha == 1f)
        {
            isToolbarOpen = false;

            // Hide Toolbar display
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(toolbarCanvasGroup, 1f, 0f, 0f, 0.3f));
            toolbarCanvasGroup.interactable = false;
            toolbarCanvasGroup.blocksRaycasts = false;

            // Lock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void HighlightInventoryBag()
    {
        new Task(ShowHighlightOnInvBag());
    }

    private IEnumerator ShowHighlightOnInvBag()
    {
        yield return new WaitForSeconds(1f);

        if (canOpenToolbar)
        {
            isToolbarOpen = true;

            // Show toolbar display
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(toolbarCanvasGroup, 0f, 1f, 0f, 0.3f));
            toolbarCanvasGroup.interactable = true;
            toolbarCanvasGroup.blocksRaycasts = true;
        }

        yield return new WaitForSeconds(0.4f);

        new Task(UIAnimation.FadeUIImageAfteDelay(inventoryBagHighlight, 0f, 1f, 0f, 0.3f));

        yield return new WaitForSeconds(2f);

        new Task(UIAnimation.FadeUIImageAfteDelay(inventoryBagHighlight, 1f, 0f, 0f, 0.1f));

        isToolbarOpen = false;

        // Hide Toolbar display
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(toolbarCanvasGroup, 1f, 0f, 0f, 0.3f));
        toolbarCanvasGroup.interactable = false;
        toolbarCanvasGroup.blocksRaycasts = false;
    }
}
