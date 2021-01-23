using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

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
    private CinemachineVirtualCamera cinemachineCamera;             // Reference to the current CinemachineCamera

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
        FindCinemachineCamera();
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
    }

    void HandleSceneChanges()
    {
        AudioManager.Initialize();
        // AudioManager.PlaySoundLooping(AudioManager.Sound.BackgroundTrack);
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
        instance.PauseGame();
        
    }

    public void QuitGame()              // Quits the game from the Pause Menu
    {
        Application.Quit();
    }

    void PauseGame()
    {
        StartCoroutine(PauseAndBounceCamera(cinemachineCamera));
    }

    private IEnumerator PauseAndBounceCamera(CinemachineVirtualCamera camera)
    {
        if (instance.pauseMenuUI)
        {
            instance.pauseMenuUI.GetComponent<Animator>().SetTrigger("Pause");
        }

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

        Cursor.visible = true;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
