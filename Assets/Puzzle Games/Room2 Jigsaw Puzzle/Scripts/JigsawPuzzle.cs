using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JigsawPuzzle : MonoBehaviour
{
    // Singleton
    public static JigsawPuzzle instance;

    // Private Cached References
    private UIReferences uiReferences;
    private Collider2D boxCollider;                                     
    private CanvasGroup jigsawpuzzleUICanvasGroup;
    private TextMeshProUGUI countdownTimerText;
    private Button interactButton;
    private TextMeshProUGUI interactText;                                   

    // Public Cached References
    public CinemachineVirtualCamera cinemachineCamera;                  // Cinemachine Camera that targets the Jigsaw Puzzle GameObject
    public GameObject jigsawPuzzleGO;                                   // Jigsaw Puzzle GameObject 
    public GameObject finalPiecesContainer;                             // Container gameObject for all the final puzzle pieces 
    public GameObject draggablePiecesContainer_1;                       // Container gameObject for all the draggable puzzle pieces on the left panel
    public GameObject draggablePiecesContainer_2;                       // Container gameObject for all the draggable puzzle pieces on the right panel
    public GameObject drawerGameObject;                                 // Gameobject of the drawer to be highlighted after successful puzzle completion

    // Private Variables
    private bool puzzleSuccess = false;
    private List<DraggablePiece> draggablePieceInstances;               // List of all the draggable puzzle piece instances
    private List<FinalPiece> finalPieceInstances;                       // List of all the final puzzle piece instances

    private TimeSpan timerTimeSpan;                                     // To format time into string
    private float timerCurrentTime = 0f;                                // Current time of the timer
    private Task timerTask;                                             // Task class variable for starting the countdown timer

    // Public Variables
    [HideInInspector]
    public bool timerStarted = false;                                   // Timer starts on pressing start button
    [HideInInspector]
    public int numberOfPiecesSolved = 0;                                // Int - Number of currently solved pieces

    public float puzzleTime = 20f;                                      // Time allowed to complete the puzzle
    [TextArea(3, 6)]
    public string[] soundHeardAfterPuzzleDialogue;                      // String array - Dialogue after sound heard of drawer opening

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

        // Check if puzzle already completed
        if (GameData.r2_jigsawPuzzleCompleted)
        {
            DisablePortraitCollider();
        }

        /*
        // Check if drawer magic potion already collected
        if (GameData.r2_drawerHealthPotionCollected)
        {
            drawerGameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        */
    }

    void SetReferences()
    {
        uiReferences = GameSession.instance.uiReferences;
        jigsawpuzzleUICanvasGroup = uiReferences.jigsawPuzzleCanvasGroup;
        countdownTimerText = uiReferences.jigsawPuzzleCountdownTimerText;
        interactButton = uiReferences.jigsawPuzzleInteractButton;
        interactText = uiReferences.jigsawPuzzleInteractText;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Initialization()
    {
        jigsawPuzzleGO.SetActive(false);
        jigsawpuzzleUICanvasGroup.alpha = 0f;
        jigsawpuzzleUICanvasGroup.interactable = false;
        jigsawpuzzleUICanvasGroup.blocksRaycasts = false;

        interactButton.onClick.AddListener(() => StartJigsawPuzzle());

        draggablePieceInstances = new List<DraggablePiece>();
        finalPieceInstances = new List<FinalPiece>();

        // Find and store all draggable piece instances
        foreach (Transform child in draggablePiecesContainer_1.transform)
        {
            draggablePieceInstances.Add(child.GetComponent<DraggablePiece>());
        }
        foreach (Transform child in draggablePiecesContainer_2.transform)
        {
            draggablePieceInstances.Add(child.GetComponent<DraggablePiece>());
        }

        // Find and store all the final piece instances
        foreach (Transform child in finalPiecesContainer.transform)
        {
            finalPieceInstances.Add(child.GetComponent<FinalPiece>());
        }
    }

    public static void LoadJigsawPuzzleUI()
    {
        instance.StartCoroutine(instance.LoadPuzzleUI());
    }

    public void DisablePortraitCollider()
    {
        boxCollider.enabled = false;
    }

    private IEnumerator LoadPuzzleUI()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Show cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor

        // Stop Player movement
        Player.StopMovement();

        // Show jigsaw puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(jigsawpuzzleUICanvasGroup, 0f, 1f, 0f, 0.2f));
        jigsawpuzzleUICanvasGroup.interactable = true;
        jigsawpuzzleUICanvasGroup.blocksRaycasts = true;

        // Set jigsaw puzzle gameobject to active
        jigsawPuzzleGO.SetActive(true);

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 15;
    }

    public void StartJigsawPuzzle()
    {
        // Disable interact button
        interactButton.interactable = false;

        EventSystem.current.SetSelectedGameObject(null);

        // Enable all colliders
        foreach (DraggablePiece instance in draggablePieceInstances)
        {
            instance.EnableCollider();
        }

        foreach (FinalPiece instance in finalPieceInstances)
        {
            instance.EnableCollider();
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

    public void AddSolvedPiece()
    {
        numberOfPiecesSolved += 1;

        if (numberOfPiecesSolved == 14)
        {
            StopTimerOnPuzzleSuccess();
        }
    }

    public void StopTimerOnPuzzleSuccess()
    {
        // Stop timer stop task
        timerTask.Stop();

        // Stop clock ticking sound
        AudioManager.StopLoopingSound(AudioManager.Sound.PuzzleClock);

        // Exit
        CloseJigsawPuzzle();
    }

    public void CloseJigsawPuzzle()
    {
        // Hide cinemachine camera for jigsaw puzzle
        StartCoroutine(ExitPuzzleGame());

        // Update this completed interaction in the GameData class
        GameData.r2_jigsawPuzzleCompleted = true;

        // Disable portrait collider
        DisablePortraitCollider();

        // Play out exit events of this puzzle
        JigsawPuzzleExitEvents();
    }

    private void JigsawPuzzleExitEvents()
    {
        // Play sound for drawer after delay
        new Task(PlayDrawerOpenSoundAfterDelay(1f));

        // Highlight health potion drawer
        new Task(HighlightHealthPotionDrawerAfterDelay(1.3f));

        // Play dialogue after sound heard
        new Task(PlayDialogueAfterDelay(3f));
    }

    private IEnumerator PlayDrawerOpenSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.R2_DrawerOpen);
    }

    private IEnumerator HighlightHealthPotionDrawerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        drawerGameObject.GetComponent<BoxCollider2D>().enabled = true;
        SpriteGlow.SpriteGlowEffect glowEffect = drawerGameObject.GetComponent<SpriteGlow.SpriteGlowEffect>();

        glowEffect.enabled = true;

        yield return new WaitForSeconds(4f);

        glowEffect.enabled = false;
    }

    private IEnumerator PlayDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (DialogueBox.instance)
        {
            DialogueBox.instance.FillSentences(soundHeardAfterPuzzleDialogue);
            DialogueBox.instance.StartDialogueDisplay();
        }
    }

    private IEnumerator ExitPuzzleGame()
    {
        LevelManager.FadeScreenInAndOut();

        yield return new WaitForSeconds(0.5f);

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor

        // Hide jigsaw puzzle gameobject
        jigsawPuzzleGO.SetActive(false);

        // Disable Player movement
        Player.StopMovement();

        // Hide jigsaw puzzle UI
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(jigsawpuzzleUICanvasGroup, 1f, 0f, 0f, 0f));
        jigsawpuzzleUICanvasGroup.interactable = false;
        jigsawpuzzleUICanvasGroup.blocksRaycasts = false;

        // Set cinemachine camera priority
        cinemachineCamera.Priority = 5;
    }

    private void ResetPuzzle()
    {
        EventSystem.current.SetSelectedGameObject(null);

        // Reset number of pieces solved variable
        numberOfPiecesSolved = 0;

        // Stop clock ticking sound
        AudioManager.StopLoopingSound(AudioManager.Sound.PuzzleClock);

        // Reset all draggable pieces to their start positions and show them
        foreach (DraggablePiece instance in draggablePieceInstances)
        {
            instance.ResetToStartPosition();
            instance.EnableSpriteRenderer();
            instance.DisableCollider();
        }

        // Hide all final pieces and enable their colliders
        foreach (FinalPiece instance in finalPieceInstances)
        {
            instance.DisableSpriteRenderer();
            instance.EnableCollider();
        }

        // Enable interact button
        interactButton.interactable = true;

        // Show retry button text
        interactText.text = "Retry";
    }
}