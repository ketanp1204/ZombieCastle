using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room3AccessBehaviour : MonoBehaviour
{
    // Singleton
    public static Room3AccessBehaviour instance;

    // Private references
    private UIReferences uiReferences;
    private TextMeshProUGUI popupTextUI;
    private SpriteGlow.SpriteGlowEffect glowEffect;

    // Public variables
    public string sceneName = "Room3";
    [TextArea(3, 6)]
    public string[] dialogueIfTorchNotAddedToInventory;
    [TextArea(3, 6)]
    public string[] dialogueIfTorchAddedButNotOiled;
    [TextArea(3, 6)]
    public string[] dialogueIfOiledTorchAvailable;

    public PC_Then_Inventory_Object lobbyTorchObject;
    public PC_Then_Inventory_Object torchWithOilObject;

    // Private variables
    private bool checkForInput = false;

    private bool torchWithOilAvailable = false;

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
        uiReferences = GameSession.instance.uiReferences;
        popupTextUI = uiReferences.popupTextUI;
        glowEffect = GetComponent<SpriteGlow.SpriteGlowEffect>();
        glowEffect.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSelectionArea"))
        {
            // Show door glow
            glowEffect.enabled = true;

            checkForInput = true;
            popupTextUI.text = "E - Open Door";
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSelectionArea"))
        {
            // Hide door glow
            glowEffect.enabled = false;

            checkForInput = false;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
        }
    }

    void Update()
    {
        if (checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Hide door glow
                glowEffect.enabled = false;

                new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

                if (DialogueBox.instance)
                { 
                    if (InventoryManager.instance)
                    {
                        if (InventoryManager.instance.ContainsItem(lobbyTorchObject))
                        {
                            DialogueBox.instance.FillSentences(dialogueIfTorchAddedButNotOiled);
                            DialogueBox.instance.SetLobbyRoom3DoorOpenFlag();
                            DialogueBox.instance.StartDialogueDisplay();

                            // Unlock barrel oil collection
                            GameData.lobby_tried_opening_r3_door_with_torch = true;
                        }
                        else
                        {
                            if (InventoryManager.instance.ContainsItem(torchWithOilObject))
                            {
                                torchWithOilAvailable = true;
                                DialogueBox.instance.FillSentences(dialogueIfOiledTorchAvailable);
                                DialogueBox.instance.SetLobbyRoom3DoorOpenFlag();
                                DialogueBox.instance.StartDialogueDisplay();
                            }
                            else
                            {
                                DialogueBox.instance.FillSentences(dialogueIfTorchNotAddedToInventory);
                                DialogueBox.instance.SetLobbyRoom3DoorOpenFlag();
                                DialogueBox.instance.StartDialogueDisplay();
                            }
                        }
                    }
                }
            }
        }
    }

    public void BehaviourAfterDialogue()
    {
        if (torchWithOilAvailable)
        {
            // Hide door glow
            glowEffect.enabled = false;

            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.DoorOpen);

            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
            LevelManager.LoadSceneByName(sceneName);
        }
    }

}
