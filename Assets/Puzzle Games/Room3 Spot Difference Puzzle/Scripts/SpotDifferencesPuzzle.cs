using System;
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
    private TextMeshProUGUI numberOfDifferencesFoundText;
    private TextMeshProUGUI countdownTimerText;
    private Button interactButton;
    private TextMeshProUGUI interactText;
    
    private List<DifferenceFound> differenceFoundScripts;

    // Public Cached References
    public CinemachineVirtualCamera cinemachineCamera;                  // Cinemachine Camera that targets the Maze Puzzle GameObject
    public GameObject puzzleGO;
    public GameObject diffCollidersContainer;
    public WeaponObject swordReward;                                    // Sword reward received after successful puzzle completion

    // Private Variables
    private int numberOfDifferencesFound = 0;
    private bool puzzleSuccess = false;

    private TimeSpan timerTimeSpan;                                     // To format time into string
    private float timerCurrentTime = 0f;                                // Current time of the timer
    private Task timerTask;                                             // Task class variable for starting the countdown timer

    // Public Variables
    [HideInInspector]
    public bool isActive = false;
    [HideInInspector]
    public bool timerStarted = false;                                   // Timer starts after 1st switch in the maze is hit

    public float puzzleTime = 35f;                                      // Time allowed to complete the puzzle
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
        uiReferences = GameSession.instance.uiReferences;
        puzzleUICanvasGroup = uiReferences.differencePuzzleCanvasGroup;
        numberOfDifferencesFoundText = uiReferences.numberOfDifferencesFoundText;
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

        // Prevent puzzle if already completed
        if (GameData.r3_spotDifferencePuzzleCompleted)
        {
            DisableCupboardCollider();
        }
    }

    public void DisableCupboardCollider()
    {
        cupboardBoxCollider.enabled = false;
    }

    public static bool IsActive()
    {
        if (instance == null)
            return false;

        return instance.isActive;
    }

    public static void LoadDiffPuzzleUI()
    {
        instance.StartCoroutine(instance.LoadPuzzle());
    }

    private IEnumerator LoadPuzzle()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Set active status
        isActive = true;

        // Show cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

        // Stop Player movement
        Player.StopMovement();
        Player.DisableAttackInput();

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
        // Play continue button sound for this start button
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.ContinueButton);

        // Disable interact button
        interactButton.interactable = false;

        EventSystem.current.SetSelectedGameObject(null);

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

    public void AddOneDifferenceFound()
    {
        // Play mouse hover sound for this click event
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryMouseHover);

        numberOfDifferencesFound += 1;

        if (numberOfDifferencesFound < 10)
        {
            char[] numberText = numberOfDifferencesFoundText.text.ToCharArray();
            char currentNumber = numberOfDifferencesFound.ToString()[0];
            numberText[0] = currentNumber;
            numberOfDifferencesFoundText.text = new string(value: numberText);
        }
        else
        {
            if (numberOfDifferencesFound == 10)
            {
                numberOfDifferencesFoundText.text = "10 of 12";
            }
            else if (numberOfDifferencesFound == 11)
            {
                numberOfDifferencesFoundText.text = "11 of 12";
            }
            else
            {
                numberOfDifferencesFoundText.text = "12 of 12";
            }
        }
        

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

        // Stop clock ticking sound
        AudioManager.StopLoopingSound(AudioManager.Sound.PuzzleClock);

        // Play success sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.MazeSuccess);

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
            DescriptionBox.instance.ShowRewardInDescBoxAfterDelay(1f, swordReward, null, AudioManager.Sound.PlayerSwordAttack);           
        }

        // Add key reward to inventory
        if (InventoryManager.instance)
        {
            InventoryManager.instance.AddInventoryItem(swordReward);
        }

        // Highlight inventory bag in toolbar
        if (ToolbarManager.instance)
        {
            ToolbarManager.instance.HighlightInventoryBag();
        }
    }

    private IEnumerator ClosePuzzleGame()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Unset active status
        isActive = false;

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor

        // Update this completed interaction in the GameData class
        GameData.r3_spotDifferencePuzzleCompleted = true;

        // Diff puzzle is inactive
        isActive = false;

        // Hide puzzle gameobject
        puzzleGO.SetActive(false);

        // Unlock lobby stairs unblock functionality
        GameData.lobby_r5_stairsUnlocked = true;

        // Enable Player movement
        Player.EnableMovement();
        Player.EnableAttackInputAfterDelay();

        // Hide maze puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(puzzleUICanvasGroup, 1f, 0f, 0f, 0f));
        puzzleUICanvasGroup.interactable = false;
        puzzleUICanvasGroup.blocksRaycasts = false;

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 5;
    }

    private void ResetPuzzle()
    {
        EventSystem.current.SetSelectedGameObject(null);

        // Reset all difference collider sprite outlines
        foreach (DifferenceFound script in differenceFoundScripts)
        {
            script.Reset();
        }

        // Stop clock ticking sound
        AudioManager.StopLoopingSound(AudioManager.Sound.PuzzleClock);

        // Reset number of differences
        numberOfDifferencesFound = 0;
        numberOfDifferencesFoundText.text = "0 of 12";

        // Stop clock ticking sound
        AudioManager.StopLoopingSound(AudioManager.Sound.PuzzleClock);

        // Enable interact button
        interactButton.interactable = true;

        // Show retry button text
        interactText.text = "Retry";
    }
}
