using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(UIReferences))]
public class GameSession : MonoBehaviour
{
    // Singleton
    public static GameSession instance;

    // Static variables
    [HideInInspector]
    public static bool GameIsPaused;                                // Is true when the game is paused

    // Private Cached References
    private GameObject pauseMenuUI;                                 // Reference to the Pause Menu UI gameObject
    private GameObject player;                                      // Reference to the Player gameObject

    // Public Cached References
    [HideInInspector]
    public UIReferences uiReferences;                                // Reference to the UIReferences GameObject
    [HideInInspector]
    public Canvas dynamicUICanvas;                                  // Reference to the DynamicUI Canvas
    [HideInInspector]
    public Camera mainCamera;                                       // Reference to the MainCamera 
    [HideInInspector]
    public GameObject objectNameGO;                                 // Reference to the object name box
    [HideInInspector]
    public GameObject dialogueManager;                              // Reference to the dialogue manager
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    // Initialize game variables and start events
    void Initialize()
    {
        GameIsPaused = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Handle events after a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetReferences();
        HandleSceneChanges();
    }

    // Initialize cached references
    void SetReferences()
    {
        player = GameObject.Find("Player");
        uiReferences = GameObject.Find("UIReferences").GetComponent<UIReferences>();
        mainCamera = uiReferences.mainCamera;
        dynamicUICanvas = uiReferences.dynamicUICanvas;
        dialogueManager = uiReferences.dialogueManager;
        pauseMenuUI = uiReferences.pauseMenuUI;
        pauseMenuUI.transform.Find("ResumeButton").gameObject.GetComponent<Button>().onClick.AddListener(() => Resume());
        pauseMenuUI.transform.Find("QuitButton").gameObject.GetComponent<Button>().onClick.AddListener(() => QuitGame());
        /*
        if (uiReferences != null)
        {
            pauseMenuUI = uiReferences.pauseMenuUI;
            pauseMenuUI.transform.Find("ResumeButton").gameObject.GetComponent<Button>().onClick.AddListener(() => Resume());
            pauseMenuUI.transform.Find("QuitButton").gameObject.GetComponent<Button>().onClick.AddListener(() => QuitGame());

            mainCamera = uiReferences.mainCamera;
            dynamicUICanvas = uiReferences.dynamicUICanvas;
            dialogueManager = uiReferences.dialogueManager;
        }
        */
    }

    void HandleSceneChanges()
    {
        AudioManager.Initialize();
        // AudioManager.PlaySoundLooping(AudioManager.Sound.BackgroundTrack);
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
    }

    public void ResumeGame()
    {
        Resume();
    }

    public static void Resume()                                 // Resume game from the pause menu
    {
        if (instance.pauseMenuUI != null)
        {
            Cursor.visible = false;
            instance.pauseMenuUI.GetComponent<Animator>().SetTrigger("Resume");
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
    }

    public static void Pause()                                  // Pauses the game when 'Escape' is pressed
    {
        if (instance.pauseMenuUI)
        {
            Cursor.visible = true;
            instance.pauseMenuUI.GetComponent<Animator>().SetTrigger("Pause");
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }

    public void QuitGame()              // Quits the game from the Pause Menu
    {
        Application.Quit();
    }
}
