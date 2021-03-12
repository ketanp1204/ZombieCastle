using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private CanvasGroup playerHealthBarCanvasGroup;

    // Public Cached References
    public CinemachineVirtualCamera cinemachineCamera;          // Cinemachine Camera that targets the Maze Puzzle GameObject

    // Private Variables
    private bool isActive;
    private bool puzzleSuccess = false;

    private TimeSpan timerTimeSpan;                             // To format time into string
    private float timerCurrentTime = 0f;                        // Current time of the timer
    private float puzzleTime = 20f;                             // Time allowed to complete the puzzle
    private float bonusTime = 5f;                               // Bonus time added after 2nd switch is hit

    private Task timerTask;                                // Task class variable for starting the countdown timer
    private Task timerStopTask;                                 // Task class variable for stopping the countdown timer

    // Public Variables
    [HideInInspector]
    public bool timerStarted = false;                           // Timer starts after 1st switch in the maze is hit

    // private bool checkForInput = false;

    private GameObject mazePuzzleGO;                            // Maze Puzzle GameObject 

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

        uiReferences = GameSession.instance.uiReferences;
        mazeUICanvasGroup = uiReferences.mazeUICanvasGroup;
        mazeSwitch1 = uiReferences.mazeSwitch1;
        mazeSwitch2 = uiReferences.mazeSwitch2;
        mazeCountdownTimerText = uiReferences.mazeCountdownTimerText;
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
    }


    // TODO: Figure out how to call this function after combat with zombies is over and player moves from left to right to go back
    public void EnablePortraitCollider()
    {
        boxCollider.enabled = true;
    }

    public void DisablePortraitCollider()
    {
        boxCollider.enabled = false;
    }

    public void ChangeSwitchSpriteToClosed(MazeSwitch.SwitchIndex switchIndex)
    {
        if (switchIndex == MazeSwitch.SwitchIndex.First)
        {
            mazeSwitch1.sprite = GameAssets.instance.switchClosedSprite;
        }
        else
        {
            mazeSwitch2.sprite = GameAssets.instance.switchClosedSprite;
        }
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
        while(timerCurrentTime > 0)
        {
            timerCurrentTime -= Time.deltaTime;
            timerTimeSpan = TimeSpan.FromSeconds(timerCurrentTime);
            string currentTimeString = timerTimeSpan.ToString("mm':'ss");
            mazeCountdownTimerText.text = currentTimeString;

            yield return new WaitForEndOfFrame();
        }

        if (!puzzleSuccess)
        {
            // Retry puzzle game
            Debug.Log("puzzle failure");

            // Temporary
            CloseMazePuzzle();
        }
    }

    public void AddBonusTime()
    {
        // Add bonus time to timer
        timerCurrentTime += bonusTime;

        // Enable exit door
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

    public void StartMazePuzzle()
    {
        // Transition to puzzle game
        StartCoroutine(LoadPuzzleGame());

        // Stop Player mvovement
        Player.StopMovement();
    }

    private IEnumerator LoadPuzzleGame()
    {
        LevelManager.FadeScreenInAndOut();

        // Hide player health bar
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(playerHealthBarCanvasGroup, 1f, 0f, 0f, 0.5f));

        yield return new WaitForSeconds(0.5f);

        // Show cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

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

        // Start movement of maze player
        MazePlayer.StartPuzzle();
    }

    // TODO: add exit behavior
    public void CloseMazePuzzle()
    {
        // Hide cinemachine camera for maze puzzle
        StartCoroutine(ClosePuzzleGame());
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
