using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{

    // Singleton
    public static GameSession instance;

    // Static variables
    [HideInInspector]
    public static bool GameIsPaused;                                // Is true when the game is paused

    // Cached References
    private UIReferences uiReferences;                              // Stores the references to the UI objects in the scene
    private GameObject pauseMenuUI;                                 // Stores the reference to the Pause Menu UI gameObject
    private GameObject player;                                      // Stores the reference to the player

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

    void Initialize()
    {
        // Initialize game variables and start events
        GameIsPaused = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Events occurring after a new scene is loaded
        SetReferences();
        HandleSceneChanges();
    }

    void SetReferences()
    {
        // Initialize cached references
        player = GameObject.Find("Player");
        uiReferences = FindObjectOfType<UIReferences>();
        if (uiReferences != null)
        {
            if(uiReferences.pauseMenuUI == null)
            {
                uiReferences.pauseMenuUI = GameObject.Find("PauseMenu");        // temporary. TODO: remove
            }
            pauseMenuUI = GameObject.Find("PauseMenu");
            pauseMenuUI.transform.Find("ResumeButton").gameObject.GetComponent<Button>().onClick.AddListener(() => Resume());
            pauseMenuUI.transform.Find("QuitButton").gameObject.GetComponent<Button>().onClick.AddListener(() => QuitGame());
        }
    }

    void HandleSceneChanges()
    {
        if(SceneManager.GetActiveScene().name == "Room1")
        {
            // Player changes   // TODO find a better way
            Vector3 scale = player.transform.localScale;
            scale.x *= 2;
            scale.y *= 2;
            player.transform.localScale = scale;
            player.transform.position = new Vector3(8.24f, 0.7f, transform.position.z);    
            player.GetComponent<Rigidbody2D>().gravityScale = 5f;
            player.GetComponent<Rigidbody2D>().mass = 200f;
            player.GetComponent<Player>().movementSpeed = 0;
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
