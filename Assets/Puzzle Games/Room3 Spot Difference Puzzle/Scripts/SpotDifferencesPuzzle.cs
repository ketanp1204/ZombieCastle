﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Cinemachine;

public class SpotDifferencesPuzzle : MonoBehaviour
{
    // Singleton
    public static SpotDifferencesPuzzle instance;

    // Private Cached References
    private UIReferences uiReferences;
    private Collider2D cupboardBoxCollider;
    private CanvasGroup puzzleUICanvasGroup;
    private TextMeshProUGUI countdownTimerText;
    private Button interactButton;
    private TextMeshProUGUI interactText;
    private GameObject puzzleGO;
    private GameObject diffCollidersContainer;
    private List<DifferenceFound> differenceFoundScripts;

    // Public Cached References
    public CinemachineVirtualCamera cinemachineCamera;                  // Cinemachine Camera that targets the Maze Puzzle GameObject
    public WeaponObject swordReward;                                    // Sword reward received after successful puzzle completion

    // Private Variables
    private bool isActive;
    private int numberOfDifferencesFound = 0;
    private bool puzzleSuccess = false;

    private TimeSpan timerTimeSpan;                                     // To format time into string
    private float timerCurrentTime = 0f;                                // Current time of the timer
    private Task timerTask;                                             // Task class variable for starting the countdown timer

    // Public Variables
    [HideInInspector]
    public bool timerStarted = false;                                   // Timer starts after 1st switch in the maze is hit

    public float puzzleTime = 20f;                                      // Time allowed to complete the puzzle
    public float bonusTime = 5f;                                        // Bonus time to add 

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

    private void SetReferences()
    {
        puzzleGO = transform.GetChild(0).gameObject;
        diffCollidersContainer = puzzleGO.transform.Find("DifferencesColliders").gameObject;

        uiReferences = GameSession.instance.uiReferences;
        puzzleUICanvasGroup = uiReferences.differencePuzzleCanvasGroup;
        countdownTimerText = uiReferences.diffPuzzleCountdownTimerText;
        interactButton = uiReferences.diffPuzzleInteractButton;
        interactText = uiReferences.diffPuzzleInteractText;

        cupboardBoxCollider = GetComponent<BoxCollider2D>();
    }

    private void Initialization()
    {
        isActive = false;
        puzzleGO.SetActive(false);
        puzzleUICanvasGroup.alpha = 0f;
        puzzleUICanvasGroup.interactable = false;
        puzzleUICanvasGroup.blocksRaycasts = false;

        differenceFoundScripts = new List<DifferenceFound>();
        foreach (Transform child in diffCollidersContainer.transform)
        {
            differenceFoundScripts.Add(child.GetComponent<DifferenceFound>());
        }

        interactButton.onClick.AddListener(() => StartDiffPuzzle());
    }

    public void DisableCupboardCollider()
    {
        cupboardBoxCollider.enabled = false;
    }

    public static void LoadDiffPuzzleUI()
    {
        instance.StartCoroutine(instance.LoadPuzzle());
    }

    private IEnumerator LoadPuzzle()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Show cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

        // Stop Player movement
        Player.StopMovement();

        // Diff puzzle is active
        isActive = true;

        // Show diff puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(puzzleUICanvasGroup, 0f, 1f, 0f, 0.2f));
        puzzleUICanvasGroup.interactable = true;
        puzzleUICanvasGroup.blocksRaycasts = true;

        // Set diff puzzle gameobject to active
        puzzleGO.SetActive(true);

        foreach (DifferenceFound script in differenceFoundScripts)
        {
            script.DisableCollider();
        }

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 15;
    }

    public void StartDiffPuzzle()
    {
        foreach (DifferenceFound script in differenceFoundScripts)
        {
            script.EnableCollider();
        }

        // Start puzzle timer
        StartPuzzleTimer();
    }

    public void StartPuzzleTimer()
    {
        timerStarted = true;
        timerCurrentTime = puzzleTime;

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

    public void AddOneDifferenceFound()
    {
        numberOfDifferencesFound += 1;

        if (numberOfDifferencesFound == 12)
        {
            StopTimerOnPuzzleSuccess();

            foreach (DifferenceFound script in differenceFoundScripts)
            {
                script.Reset();
            }
        }
    }

    public void StopTimerOnPuzzleSuccess()
    {
        // Stop timer stop task
        timerTask.Stop();

        // Exit
        CloseDiffPuzzle();
    }

    public void CloseDiffPuzzle()
    {
        // Hide cinemachine camera for maze puzzle
        StartCoroutine(ClosePuzzleGame());

        // Disable portrait collider
        DisableCupboardCollider();

        // Show key reward 
        if (DescriptionBox.instance)
        {
            DescriptionBox.instance.ShowDescBoxAfterReward(swordReward, null);
        }

        // Add key reward to inventory
        if (InventoryManager.instance)
        {
            InventoryManager.instance.AddInventoryItem(swordReward);
        }
    }

    private IEnumerator ClosePuzzleGame()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor

        // Diff puzzle is inactive
        isActive = false;

        // Hide puzzle gameobject
        puzzleGO.SetActive(false);

        // Enable Player movement
        Player.EnableMovement();

        // Hide maze puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(puzzleUICanvasGroup, 1f, 0f, 0f, 0f));
        puzzleUICanvasGroup.interactable = false;
        puzzleUICanvasGroup.blocksRaycasts = false;

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 5;
    }

    public static bool IsActive()
    {
        return instance.isActive;
    }

    private void ResetPuzzle()
    {
        EventSystem.current.SetSelectedGameObject(null);

        numberOfDifferencesFound = 0;

        // Reset all difference collider sprite outlines
        foreach (DifferenceFound script in differenceFoundScripts)
        {
            script.Reset();
        }

        // Show retry button text
        interactText.text = "Retry";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
