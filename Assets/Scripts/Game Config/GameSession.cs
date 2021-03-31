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

    public WeaponObject knifeInventoryObject;

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
        
        // Time.timeScale = 0.4f;          // Testing
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
            // Play background track
            AudioManager.PlaySoundLooping(AudioManager.Sound.BackgroundTrack);

            // Stop room 3 background track
            AudioManager.StopLoopingSound(AudioManager.Sound.Room3Background);

            // Handle first scene events
            if (PlayerStats.isFirstScene)
            {
                // Play intro dialogue 
                if (!GameData.lobby_introDialogueSeen)
                    StartCoroutine(PlayIntroDialogue());
            }

            // Play candle burning sound
            AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.CandleBurning);

            // Hide cursor
            Cursor.lockState = CursorLockMode.Locked;

            new Task(DisableInventoryWeaponInteractionAfterDelay(1f));
        }
        else if (scene.name == "Room3")
        {
            // Play room3 background sound
            AudioManager.PlaySoundLooping(AudioManager.Sound.Room3Background);

            // Play candle burning sound
            AudioManager.PlaySoundLooping(AudioManager.Sound.CandleBurning);

            // Stop game background sound
            AudioManager.StopLoopingSound(AudioManager.Sound.BackgroundTrack);
        }
        else if (scene.name == "Room5")
        {
            // Play room5 background sound
            AudioManager.PlaySoundLooping(AudioManager.Sound.Room5Background);

            // Stop game background sound
            AudioManager.StopLoopingSound(AudioManager.Sound.BackgroundTrack);

            // Stop candle burning sound
            AudioManager.StopLoopingSound(AudioManager.Sound.CandleBurning);
        }
        else
        {
            // Play background track
            AudioManager.PlaySoundLooping(AudioManager.Sound.BackgroundTrack);

            // Stop room 3 background track
            AudioManager.StopLoopingSound(AudioManager.Sound.Room3Background);

            // Stop candle burning sound
            AudioManager.StopLoopingSound(AudioManager.Sound.CandleBurning);
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

        GameData.lobby_introDialogueSeen = true;

        yield return new WaitForSeconds(4f);

        // Reset LevelManager animation speed
        LevelManager.SetAnimatorSpeed(1f);
    }

    private IEnumerator DisableInventoryWeaponInteractionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (InventoryManager.instance)
        {
            InventoryManager.instance.DisableAllWeaponInteractions();
        }
    }

    private void OnApplicationQuit()
    {
        GameData.currentPlayerInventory.Container.Clear();
        GameData.currentPlayerInventory.AddItem(knifeInventoryObject, 1);
    }
}