﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Cinemachine;

public class MazePuzzle : MonoBehaviour
{
    // Singleton
    public static MazePuzzle instance;

    // Private Cached References
    private UIReferences uiReferences;
    private Collider2D boxCollider;
    private CanvasGroup mazeUICanvasGroup;
    private Image mazeSwitch1;
    private Image mazeSwitch2;
    private TextMeshProUGUI countdownTimerText;
    private Button interactButton;
    private TextMeshProUGUI interactText;
    

    // Public Cached References
    public CinemachineVirtualCamera cinemachineCamera;                  // Cinemachine Camera that targets the Maze Puzzle GameObject
    public GameObject mazePuzzleGO;                                     // Maze Puzzle GameObject 
    public Collider2D switch1Collider;                                  // Switch 1 collider
    public Collider2D switch2Collider;                                  // Switch 2 collider
    public PC_Then_Inventory_Object keyRewardScriptableObject;          // Key reward received after successful puzzle completion    

    // Private Variables
    private bool puzzleSuccess = false;

    private TimeSpan timerTimeSpan;                                     // To format time into string
    private float timerCurrentTime = 0f;                                // Current time of the timer
    private Task timerTask;                                             // Task class variable for starting the countdown timer

    // Public Variables
    [HideInInspector]
    public bool isActive = false;
    [HideInInspector]
    public bool firstSwitchHitFlag = false;                             // Set after 1st switch is hit
    [HideInInspector]
    public bool timerStarted = false;                                   // Timer starts after 1st switch in the maze is hit
    public float puzzleTime = 20f;                                      // Time allowed to complete the puzzle
    public float bonusTime = 5f;                                        // Bonus time added after each switch is hit

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
        Initialization();
    }

    void SetReferences()
    {
        uiReferences = GameSession.instance.uiReferences;
        mazeUICanvasGroup = uiReferences.mazeUICanvasGroup;
        mazeSwitch1 = uiReferences.mazeSwitch1;
        mazeSwitch2 = uiReferences.mazeSwitch2;
        countdownTimerText = uiReferences.mazeCountdownTimerText;
        interactButton = uiReferences.mazePuzzleInteractButton;
        interactText = uiReferences.mazePuzzleInteractText;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Initialization()
    {
        mazePuzzleGO.SetActive(false);
        mazeUICanvasGroup.alpha = 0f;
        mazeUICanvasGroup.interactable = false;
        mazeUICanvasGroup.blocksRaycasts = false;

        interactButton.onClick.AddListener(() => StartMazePuzzle());

        if (GameData.r1_combatCompleted)
        {
            if (GameData.r1_mazePuzzleCompleted)
            {
                DisablePortraitCollider();
            }
            else
            {
                EnablePortraitCollider();
            }
        }
        else
        {
            DisablePortraitCollider();
        }
    }

    public void EnablePortraitCollider()
    {
        boxCollider.enabled = true;
    }

    public void DisablePortraitCollider()
    {
        boxCollider.enabled = false;
    }

    public static bool IsActive()
    {
        if (instance == null)
            return false;

        return instance.isActive;
    }

    public static void LoadMazePuzzleUI()
    {
        instance.StartCoroutine(instance.LoadPuzzleUI());
    }

    private IEnumerator LoadPuzzleUI()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        AudioManager.SetLoopingSoundVolume(AudioManager.Sound.BackgroundTrack, 0.02f);

        // Set active status
        isActive = true;

        // Show cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

        // Stop Player movement
        Player.StopMovement();
        Player.DisableAttackInput();

        // Show maze puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(mazeUICanvasGroup, 0f, 1f, 0f, 0.2f));
        mazeUICanvasGroup.interactable = true;
        mazeUICanvasGroup.blocksRaycasts = true;

        // Set maze puzzle gameobject to active
        mazePuzzleGO.SetActive(true);

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 15;

        // Set maze player start position
        MazePlayer.SetStartPosition();
    }

    public void StartMazePuzzle()
    {
        // Start movement of maze player
        MazePlayer.StartPuzzle();

        // Disable interact button
        interactButton.interactable = false;
        EventSystem.current.SetSelectedGameObject(null);

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor

        // Start puzzle timer
        StartPuzzleTimer();
    }

    public void StartPuzzleTimer()
    {
        timerStarted = true;
        timerCurrentTime = puzzleTime;

        // Start playing clock ticking sound
        AudioManager.PlaySoundLooping(AudioManager.Sound.PuzzleClock);

        // Start updating the timer
        timerTask = new Task(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (timerCurrentTime > 0)
        {
            timerCurrentTime -= Time.deltaTime;
            timerTimeSpan = TimeSpan.FromSeconds(timerCurrentTime);
            string currentTimeString = timerTimeSpan.ToString("mm':'ss");
            countdownTimerText.text = currentTimeString;

            yield return new WaitForEndOfFrame();
        }

        // Puzzle failure
        if (!puzzleSuccess)
        {
            // Retry puzzle game
            ResetPuzzle();
        }
    }

    public void AddBonusTime()
    {
        // Add bonus time to timer
        timerCurrentTime += bonusTime;

        // Enable exit door if 2nd switch hit
        if (firstSwitchHitFlag)
            MazeExitDoor.instance.EnableExit();
    }

    public void StopTimerOnPuzzleSuccess()
    {
        // Stop timer stop task
        timerTask.Stop();

        // Stop clock ticking sound
        AudioManager.StopLoopingSound(AudioManager.Sound.PuzzleClock);

        // Play success sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.MazeSuccess);

        // Exit
        CloseMazePuzzle();
    }


    public void ChangeSwitchSpriteToOn(MazeSwitch.SwitchIndex switchIndex)
    {
        if (switchIndex == MazeSwitch.SwitchIndex.First)
        {
            mazeSwitch1.sprite = GameAssets.instance.switchOnSprite;
        }
        else
        {
            mazeSwitch2.sprite = GameAssets.instance.switchOnSprite;
        }
    }

    public void CloseMazePuzzle()
    {
        // Go back to game
        StartCoroutine(ExitPuzzleGame());

        // Update this completed interaction in the GameData class
        GameData.r1_mazePuzzleCompleted = true;

        // Disable portrait collider
        DisablePortraitCollider();

        // Show key reward 
        if (DescriptionBox.instance)
        {
            DescriptionBox.instance.ShowRewardInDescBoxAfterDelay(1f, keyRewardScriptableObject, null, AudioManager.Sound.KeyCollect);
        }
    }

    private IEnumerator ExitPuzzleGame()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Unset active status
        isActive = false;

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor

        AudioManager.SetLoopingSoundVolume(AudioManager.Sound.BackgroundTrack, 0.04f);

        // Hide maze puzzle gameobject
        mazePuzzleGO.SetActive(false);

        // Enable Player movement
        Player.EnableMovement();
        Player.EnableAttackInputAfterDelay();

        // Stop movement of maze player
        MazePlayer.EndPuzzle();

        // Hide maze puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(mazeUICanvasGroup, 1f, 0f, 0f, 0f));
        mazeUICanvasGroup.interactable = false;
        mazeUICanvasGroup.blocksRaycasts = false;

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 5;
    }

    private void ResetPuzzle()
    {
        EventSystem.current.SetSelectedGameObject(null);

        // Show cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

        // Stop clock ticking sound
        AudioManager.StopLoopingSound(AudioManager.Sound.PuzzleClock);

        // Stop maze player movement
        MazePlayer.EndPuzzle();

        // Reset puzzle sprites
        ResetSprites();

        // Enable switch colliders
        switch1Collider.enabled = true;
        switch2Collider.enabled = true;

        // Unset first switch hit flag
        firstSwitchHitFlag = false;

        // Enable interact button
        interactButton.interactable = true;

        // Show retry button text
        interactText.text = "Retry";
    }

    private void ResetSprites()
    {
        mazeSwitch1.sprite = GameAssets.instance.switchOffSprite;
        mazeSwitch2.sprite = GameAssets.instance.switchOffSprite;
        MazeExitDoor.instance.EnableDoorSprite();
    }
}
