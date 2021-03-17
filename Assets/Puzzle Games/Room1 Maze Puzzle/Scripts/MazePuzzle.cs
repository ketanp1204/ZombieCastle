using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Cinemachine;

public class MazePuzzle : MonoBehaviour
{
    // Instance
    public static MazePuzzle instance;

    // Private Cached References
    private UIReferences uiReferences;
    private Collider2D boxCollider;
    private CanvasGroup mazeUICanvasGroup;
    private Image mazeSwitch1;
    private Image mazeSwitch2;
    private TextMeshProUGUI mazeCountdownTimerText;
    private Button interactButton;
    private TextMeshProUGUI interactText;
    private GameObject mazePuzzleGO;                            // Maze Puzzle GameObject 
    private Collider2D switch1Collider;                         // Switch 1 collider
    private Collider2D switch2Collider;                         // Switch 2 collider
    private CanvasGroup playerHealthBarCanvasGroup;

    // Public Cached References
    public CinemachineVirtualCamera cinemachineCamera;          // Cinemachine Camera that targets the Maze Puzzle GameObject

    // Private Variables
    private bool isActive;
    private bool puzzleSuccess = false;

    private TimeSpan timerTimeSpan;                             // To format time into string
    private float timerCurrentTime = 0f;                        // Current time of the timer
    private float puzzleTime = 20f;                             // Time allowed to complete the puzzle
    private float bonusTime = 5f;                               // Bonus time added after each switch is hit

    private Task timerTask;                                     // Task class variable for starting the countdown timer

    // Public Variables
    [HideInInspector]
    public bool firstSwitchHitFlag = false;                       // Set after 1st switch is hit
    [HideInInspector]
    public bool timerStarted = false;                           // Timer starts after 1st switch in the maze is hit

    // private bool checkForInput = false;

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
        mazePuzzleGO = transform.GetChild(0).gameObject;
        switch1Collider = mazePuzzleGO.transform.Find("Switch1Collider").GetComponent<BoxCollider2D>();
        switch2Collider = mazePuzzleGO.transform.Find("Switch2Collider").GetComponent<BoxCollider2D>();

        uiReferences = GameSession.instance.uiReferences;
        mazeUICanvasGroup = uiReferences.mazeUICanvasGroup;
        mazeSwitch1 = uiReferences.mazeSwitch1;
        mazeSwitch2 = uiReferences.mazeSwitch2;
        mazeCountdownTimerText = uiReferences.mazeCountdownTimerText;
        interactButton = uiReferences.interactButton;
        interactText = uiReferences.interactText;

        playerHealthBarCanvasGroup = uiReferences.playerHealthBarCanvasGroup;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Initialization()
    {
        isActive = false;
        mazePuzzleGO.SetActive(false);
        // boxCollider.enabled = false;
        mazeUICanvasGroup.alpha = 0f;
        mazeUICanvasGroup.interactable = false;
        mazeUICanvasGroup.blocksRaycasts = false;

        interactButton.onClick.AddListener(() => StartMazePuzzle());
    }

    public void DisablePortraitCollider()
    {
        boxCollider.enabled = false;
    }

    public static void LoadMazePuzzleUI()
    {
        instance.StartCoroutine(instance.LoadPuzzleUI());
    }

    private IEnumerator LoadPuzzleUI()
    {
        LevelManager.FadeScreenInAndOut();

        // Hide player health bar
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerHealthBarCanvasGroup, 1f, 0f, 0f, 0.5f));

        yield return new WaitForSeconds(0.5f);

        // Show cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

        // Stop Player movement
        Player.StopMovement();

        // Maze puzzle is active
        isActive = true;

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

        // Start puzzle timer
        StartPuzzleTimer();
    }

    public void StartPuzzleTimer()
    {
        Debug.Log("puzzle timer started");
        timerStarted = true;
        timerCurrentTime = puzzleTime;

        // Start updating the timer
        timerTask = new Task(UpdateTimer());

        // Start a coroutine to stop the timer after intial time given to complete the puzzle
        // timerStopTask = new Task(StopTimerAfterSeconds(puzzleTime));
    }

    private IEnumerator UpdateTimer()
    {
        while (timerCurrentTime > 0)
        {
            timerCurrentTime -= Time.deltaTime;
            timerTimeSpan = TimeSpan.FromSeconds(timerCurrentTime);
            string currentTimeString = timerTimeSpan.ToString("mm':'ss");
            mazeCountdownTimerText.text = currentTimeString;

            yield return new WaitForEndOfFrame();
        }

        // Puzzle failure
        if (!puzzleSuccess)
        {
            // Retry puzzle game
            Debug.Log("puzzle failure");

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

        // Show reward 
        Debug.Log("puzzle success");

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

    // TODO: add exit behavior
    public void CloseMazePuzzle()
    {
        // Hide cinemachine camera for maze puzzle
        StartCoroutine(ClosePuzzleGame());

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 5;

        // Disable portrait collider
        DisablePortraitCollider();
    }

    private IEnumerator ClosePuzzleGame()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Show player health bar
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerHealthBarCanvasGroup, 0f, 1f, 0f, 0.5f));

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor

        // Maze puzzle is active
        isActive = false;

        // Hide maze puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(mazeUICanvasGroup, 1f, 0f, 0f, 0.2f));
        mazeUICanvasGroup.interactable = false;
        mazeUICanvasGroup.blocksRaycasts = false;

        // Hide maze puzzle gameobject
        mazePuzzleGO.SetActive(false);

        // Enable Player movement
        Player.EnableMovement();

        // Stop movement of maze player
        MazePlayer.EndPuzzle();
    }

    public static bool IsActive()
    {
        return instance.isActive;
    }

    private void ResetPuzzle()
    {
        EventSystem.current.SetSelectedGameObject(null);

        // Stop maze player movement
        MazePlayer.EndPuzzle();

        // Reset puzzle sprites
        ResetSprites();

        // Enable switch colliders
        switch1Collider.enabled = true;
        switch2Collider.enabled = true;

        // Unset first switch hit flag
        firstSwitchHitFlag = false;

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
