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

    // REMOVE BEFORE RELEASING GAME
    public WeaponObject knifeInventoryObject;                       // Temporary - Used to re add the knife to the default player inventory after exiting play in the unity editor

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
        if (scene.name == "CastleLobby")
        {
            // Handle first scene events
            if (PlayerStats.isFirstScene)
            {
                // Initialize Game Data
                GameData.Initialize();

                // Play intro dialogue
                StartCoroutine(PlayIntroDialogue());
            }

            // Play candle burning sound
            AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.CandleBurning);

            // Hide cursor
            Cursor.lockState = CursorLockMode.Locked;

            // Destroy key and torch gameobjects if already collected
            if (GameData.lobby_keyCollected)
            {
                Destroy(GameObject.Find("Key"));
                Destroy(GameObject.Find("Key_Image"));
            }

            if (GameData.lobby_torchCollected)
            {
                Destroy(GameObject.Find("Torch"));
                Destroy(GameObject.Find("Torch_Image"));
            }
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
            DialogueBox.instance.SetGameStartDialogueFlag();
            DialogueBox.instance.StartDialogueDisplay();
        }

        yield return new WaitForSeconds(4f);

        // Reset LevelManager animation speed
        LevelManager.SetAnimatorSpeed(1f);
    }

    // REMOVE BEFORE RELEASING GAME
    private void OnApplicationQuit()
    {
        GameData.currentPlayerInventory.Container.Clear();
        GameData.currentPlayerInventory.AddItem(knifeInventoryObject, 1);
    }
}