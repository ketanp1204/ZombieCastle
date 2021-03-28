using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // Singleton
    public static SettingsMenu instance;

    public Button controlsDisplayButton;
    public Button closeButton;

    private CanvasGroup canvasGroup;

    // Private variables
    private bool isSettingsMenuOpen = false;
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

        if (ControlsDisplay.instance)
        {
            controlsDisplayButton.onClick.AddListener(() => ControlsDisplay.instance.ShowControlsDisplay());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSettingsMenuOpen && !isControlsDisplayOpen)
            {
                HideSettingsMenu();
            }
        }
    }

    public void ShowSettingsMenu()
    {
        isSettingsMenuOpen = true;

        // Show settings menu display
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(canvasGroup, 0f, 1f, 0f, 0.2f));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Unlock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;

        // Pause game time
        Time.timeScale = 0f;

        // Disable opening pause menu on pressing escape key
        PauseMenu.instance.CanPauseGame = false;
    }

    public void HideSettingsMenu()
    {
        isSettingsMenuOpen = false;

        // Hide settings menu display
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(canvasGroup, 1f, 0f, 0f, 0.2f));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Lock mouse cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Resume game time
        Time.timeScale = 1f;

        // Enable opening pause menu on pressing escape key
        StartCoroutine(PauseMenu.EnableCanPauseGameBoolAfterDelay(0.1f));
    }

    public void LoadLastCheckpoint()
    {
        GameOverScreen.LoadLastCheckpoint();

        HideSettingsMenu();
    }

    public void SetControlsDisplayOpenFlag()
    {
        isControlsDisplayOpen = true;
    }

    public void UnsetControlsDisplayOpenFlag()
    {
        isControlsDisplayOpen = false;
    }

    public void HideCloseButton()
    {
        closeButton.gameObject.SetActive(false);
    }

    public void ShowCloseButton()
    {
        closeButton.gameObject.SetActive(true);
    }
}
