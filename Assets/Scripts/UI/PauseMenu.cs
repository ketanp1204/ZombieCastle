﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;

public class PauseMenu : MonoBehaviour
{
    // Singleton
    public static PauseMenu instance;

    // Public references
    public Button resumeButton;
    public Button quitButton;

    // Public static variables
    [HideInInspector]
    public static bool GameIsPaused;                                // Is true when the game is paused

    // Public variables
    [HideInInspector]
    public bool CanPauseGame;                                       // Is true if no UI object that can be closed by pressing escape is open

    // Private cached references
    private Animator animator;
    private CinemachineVirtualCamera cinemachineCamera;             // Reference to the current CinemachineCamera

    private Button currentlySelectedButton = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        SetReferences();
        FindCinemachineCamera();

        GameIsPaused = false;
        CanPauseGame = true;
    }

    private void SetReferences()
    {
        animator = GetComponent<Animator>();
    }

    void FindCinemachineCamera()
    {
        int priority = -1;
        CinemachineVirtualCamera[] cCams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (CinemachineVirtualCamera camera in cCams)
        {
            if (camera.Priority > priority)
            {
                priority = camera.Priority;
                cinemachineCamera = camera;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))                   // Pauses the game on pressing 'Escape'
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (GameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentlySelectedButton == null)
                {
                    currentlySelectedButton = resumeButton;
                    EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
                }
                else if (currentlySelectedButton == resumeButton)
                {
                    currentlySelectedButton = quitButton;
                    EventSystem.current.SetSelectedGameObject(quitButton.gameObject);
                }
                else if (currentlySelectedButton == quitButton)
                {
                    currentlySelectedButton = resumeButton;
                    EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentlySelectedButton == null)
                {
                    currentlySelectedButton = quitButton;
                    EventSystem.current.SetSelectedGameObject(quitButton.gameObject);
                }
                else if (currentlySelectedButton == quitButton)
                {
                    currentlySelectedButton = resumeButton;
                    EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
                }
                else if (currentlySelectedButton == resumeButton)
                {
                    currentlySelectedButton = quitButton;
                    EventSystem.current.SetSelectedGameObject(quitButton.gameObject);
                }
            }
        }
    }

    public static void Resume()                                                 // Resume game from the pause menu
    {
        /* NOTE:    In the Unity editor, the Resume function does not work as intended. 
         *          On clicking on the resume button in the pause menu to resume the game, the mouse cursor disappears as expected.
         *          But on pressing escape from pause menu to resume the game, the mouse cursor does not disappear.
         *          However, in the game build, this function works as expected when pressing escape to resume.
        */

        if (GameIsPaused)
        {
            if (!DialogueBox.IsOpen() && !NoteBox.IsOpen() && !DescriptionBox.IsOpen() && !MazePuzzle.IsActive() && !JigsawPuzzle.IsActive() && !SpotDifferencesPuzzle.IsActive())                                          
                Cursor.lockState = CursorLockMode.Locked;                       // Center and lock mouse cursor

            AudioListener.pause = false;
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.ContinueButton);
            instance.GetComponent<Animator>().SetTrigger("Resume");
            Time.timeScale = 1f;
            GameIsPaused = false;
            EventSystem.current.SetSelectedGameObject(null);

            Player.EnableMovement();
            Player.EnableAttackInputAfterDelay();
        }
    }

    public static void Pause()                                                  // Pauses the game when 'Escape' is pressed
    {
        if (!GameIsPaused && instance.CanPauseGame)
        {
            if (Player.instance)
            {
                Player.StopMovement();
                Player.DisableAttackInput();
            }
            instance.PauseGame();
        }
            
    }

    public static void QuitGame()                                               // Quits the game from the Pause Menu
    {
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.ContinueButton);
        Application.Quit();
    }

    void PauseGame()
    {
        StartCoroutine(PauseAndBounceCamera(cinemachineCamera));
    }

    private IEnumerator PauseAndBounceCamera(CinemachineVirtualCamera camera)
    {
        animator.SetTrigger("Pause");

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;

        float orthographicSize = camera.m_Lens.OrthographicSize;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / 0.1f;

            float currentValue = Mathf.Lerp(orthographicSize, orthographicSize - 0.13f, percentageComplete);

            camera.m_Lens.OrthographicSize = currentValue;

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        _timeStartedLerping = Time.time;
        orthographicSize = camera.m_Lens.OrthographicSize;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / 0.2f;

            float currentValue = Mathf.Lerp(orthographicSize, orthographicSize + 0.13f, percentageComplete);

            camera.m_Lens.OrthographicSize = currentValue;

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public static IEnumerator EnableCanPauseGameBoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.CanPauseGame = true;
    }
}
