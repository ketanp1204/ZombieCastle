using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    // Singleton
    public static GameSession instance;

    // Public cached references
    [HideInInspector]
    public UIReferences uiReferences;                               // Global reference for the UIReferences script in the current scene
    
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

        // Time.timeScale = 0.2f;          // Testing
    }

    // Initialize cached references
    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
    }

    void HandleSceneChanges()
    {
        AudioManager.Initialize();
        // AudioManager.PlayNewSoundLooping(AudioManager.Sound.BackgroundTrack);
    }

    public static void ResetPlayerStats()
    {
        PlayerStats.isFirstScene = true;
        PlayerStats.IsDead = false;
    }
}