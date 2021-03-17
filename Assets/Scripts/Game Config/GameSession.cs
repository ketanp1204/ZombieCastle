using System.Collections;
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

        PlayerStats.Initialize();
        AudioManager.Initialize();
        AudioManager.PlaySoundLooping(AudioManager.Sound.BackgroundTrack);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Handle events after a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetReferences();
        HandleSceneChanges(scene);
        
        // Time.timeScale = 0.2f;          // Testing
    }

    // Initialize cached references
    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
    }

    void HandleSceneChanges(Scene scene)
    {
        // Time.timeScale = 0.2f;           // Testing
        
        if (scene.name == "CastleLobby" && PlayerStats.isFirstScene)
        {
            // Play candle burning sound
            AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.CandleBurning);

            if (PlayerStats.isFirstScene)
                StartCoroutine(PlayIntroDialogue());
        }
    }

    public static void ResetPlayerStats()
    {
        PlayerStats.isFirstScene = true;
        PlayerStats.IsDead = false;
    }

    private IEnumerator PlayIntroDialogue()
    {
        yield return new WaitForSeconds(3f);

        // Play start dialogue
        if (DialogueBox.instance)
        {
            DialogueBox.instance.FillSentences(GameAssets.instance.gameStartDialogue);
            DialogueBox.instance.StartDialogueDisplay();
        }

        yield return new WaitForSeconds(4f);

        // Reset LevelManager animation speed
        LevelManager.SetAnimatorSpeed(1f);
    }
}