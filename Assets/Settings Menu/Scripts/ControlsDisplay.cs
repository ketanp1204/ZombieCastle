using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsDisplay : MonoBehaviour
{
    // Singleton
    public static ControlsDisplay instance;

    
    // Private references
    private CanvasGroup canvasGroup;

    // Private variables
    private bool isControlsDisplayOpen = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isControlsDisplayOpen)
            {
                HideControlsDisplay();
            }
        }
    }

    public void ShowControlsDisplay()
    {
        isControlsDisplayOpen = true;

        new Task(UIAnimation.FadeCanvasGroupAfterDelay(canvasGroup, 0f, 1f, 0f, 0.2f));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideControlsDisplay()
    {
        isControlsDisplayOpen = false;

        // Show settings menu close button
        SettingsMenu.instance.ShowCloseButton();

        // Unset controls display flag on settings menu
        SettingsMenu.instance.UnsetControlsDisplayOpenFlag();

        new Task(UIAnimation.FadeCanvasGroupAfterDelay(canvasGroup, 1f, 0f, 0f, 0.2f));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
