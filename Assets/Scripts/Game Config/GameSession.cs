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

    // REMOVE BEFORE RELEASING GAME - Temporary - Used to re add the scriptable objects to the default player inventory after exiting play in the unity editor
    public PC_Then_Inventory_Object lobbyKeyInventoryObject;        
    public PC_Then_Inventory_Object lobbyTorchInventoryObject;
    public PC_Then_Inventory_Object r1_oil_barrel;
    public DescBox_Then_Dialogue_Object fireElement;
    public DescBox_Then_Dialogue_Object magicPotion;
    public WeaponObject sword;

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
        // AudioManager.PlaySoundLooping(AudioManager.Sound.BackgroundTrack);
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
        
        Time.timeScale = 0.4f;          // Testing
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
                if(!GameData.isInitialized)
                    GameData.Initialize();

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
        GameData.currentPlayerInventory.AddItem(sword, 1);
        /*
        GameData.currentPlayerInventory.AddItem(lobbyKeyInventoryObject, 1);
        GameData.currentPlayerInventory.AddItem(lobbyTorchInventoryObject, 1);
        GameData.currentPlayerInventory.AddItem(r1_oil_barrel, 1);
        GameData.currentPlayerInventory.AddItem(fireElement, 1);
        GameData.currentPlayerInventory.AddItem(magicPotion, 1);
        
        */
    }
}