using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBoxInteraction : MonoBehaviour
{
    public enum RoomIndex
    {
        Room1,
        Room2,
        Room3
    }

    // Public references
    public GameObject imageDisplayGO;


    // Private references
    private BoxCollider2D boxCollider;
    private SpriteGlow.SpriteGlowEffect glowEffect;


    // Room 1 
    [Header("Room 1")]
    public PC_Then_Inventory_Object lobbyKeyScriptableObject;
    public WeaponObject axeScriptableObject;
    [TextArea(3, 6)]
    public string[] dialogueIfLobbyKeyCollected;
    [TextArea(3, 6)]
    public string[] dialogueIfLobbyKeyNotCollected;
    [TextArea(3, 6)]
    public string[] axeReceivedDialogue;

    // Room 2
    [Header("Room 2")]
    public PC_Then_Inventory_Object room2TreasureBoxKeyScriptableObject;
    public DescBox_Then_Dialogue_Object magicPotionScriptableObject;
    [TextArea(3, 6)]
    public string[] dialogueIfKeyFromMazeCollected;
    [TextArea(3, 6)]
    public string[] dialogueIfKeyFromMazeNotCollected;

    // Room 3
    [Header("Room 3")]
    public DescBox_Then_Dialogue_Object fireElementScriptableObject;
    [TextArea(3, 6)]
    public string[] dialogueBeforeReceivingFireElement;



    [Header("Config")]
    // Public variables
    public RoomIndex roomIndex;


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        glowEffect = GetComponent<SpriteGlow.SpriteGlowEffect>();
        glowEffect.enabled = false;

        // Check if treasure box has been already interacted with
        if (roomIndex == RoomIndex.Room1)
        {
            if (GameData.r1_treasureBoxAxeCollected)
            {
                SetTreasureBoxSpriteToOpenAndDisableCollider();
            }
        }
        else if (roomIndex == RoomIndex.Room2)
        {
            if (GameData.r2_treasureBoxMagicPotionCollected)
            {
                SetTreasureBoxSpriteToOpenAndDisableCollider();
            }
        }
        else if (roomIndex == RoomIndex.Room3)
        {
            if (GameData.r3_treasureBoxFireElementCollected)
            {
                SetTreasureBoxSpriteToOpenAndDisableCollider();
            }
        }
    }

    private void SetTreasureBoxSpriteToOpenAndDisableCollider()
    {
        imageDisplayGO.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.treasureBoxOpenSprite;
        DisableCollider();
    }

    public void StartInteractionBehaviour()
    {
        if (roomIndex == RoomIndex.Room1)
        {
            if (InventoryManager.instance)
            {
                // Chcek if player has collected the key from the lobby
                if (InventoryManager.instance.ContainsItem(lobbyKeyScriptableObject))
                {
                    if (DialogueBox.instance)
                    {
                        DialogueBox.instance.FillSentences(dialogueIfLobbyKeyCollected);
                        DialogueBox.instance.SetTreasureBoxFlag();
                        DialogueBox.instance.SetTreasureBoxScript(this);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                }
                else
                {
                    if (DialogueBox.instance)
                    {
                        DialogueBox.instance.FillSentences(dialogueIfLobbyKeyNotCollected);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                }
            }
        }
        else if (roomIndex == RoomIndex.Room2)
        {
            // Check if player has collected the key from the maze puzzle
            if (InventoryManager.instance.ContainsItem(room2TreasureBoxKeyScriptableObject))
            {
                if (DialogueBox.instance)
                {
                    DialogueBox.instance.FillSentences(dialogueIfKeyFromMazeCollected);
                    DialogueBox.instance.SetTreasureBoxFlag();
                    DialogueBox.instance.SetTreasureBoxScript(this);
                    DialogueBox.instance.StartDialogueDisplay();
                }
            }
            else
            {
                if (DialogueBox.instance)
                {
                    DialogueBox.instance.FillSentences(dialogueIfKeyFromMazeNotCollected);
                    DialogueBox.instance.StartDialogueDisplay();
                }
            }
        }
        else if (roomIndex == RoomIndex.Room3)
        {
            if (DialogueBox.instance)
            {
                DialogueBox.instance.FillSentences(dialogueBeforeReceivingFireElement);
                DialogueBox.instance.SetTreasureBoxFlag();
                DialogueBox.instance.SetTreasureBoxScript(this);
                DialogueBox.instance.StartDialogueDisplay();
            }
        }
    }

    public void BehaviourAfterDialogue()
    {
        if (roomIndex == RoomIndex.Room1)
        {
            // Open inventory to select key
            InventoryManager.ShowInventory();
            InventoryManager.instance.HighlightItemOnTreasureBoxInteraction(lobbyKeyScriptableObject, this);
        }
        else if (roomIndex == RoomIndex.Room2)
        {
            // Open inventory to select key
            InventoryManager.ShowInventory();
            InventoryManager.instance.HighlightItemOnTreasureBoxInteraction(room2TreasureBoxKeyScriptableObject, this);
        }
        else if (roomIndex == RoomIndex.Room3)
        {
            // Show fire element description box
            if (DescriptionBox.instance)
            {
                DescriptionBox.instance.ShowRewardInDescBoxAfterDelay(0.3f, fireElementScriptableObject, fireElementScriptableObject.playerComments, AudioManager.Sound.FireElement);

                // Update this completed interaction in the GameData class
                GameData.r3_treasureBoxFireElementCollected = true;

                // Disable the treasure box's collider
                DisableCollider();
            }
        }
    }

    public void BehaviourAfterInventoryItemSelected()
    {
        if (roomIndex == RoomIndex.Room1)
        {
            // Set treasure box image display to box open sprite
            imageDisplayGO.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.treasureBoxOpenSprite;

            // Show axe received on description box and following dialogue
            if (DescriptionBox.instance)
            {
                DescriptionBox.instance.ShowRewardInDescBoxAfterDelay(0.3f, axeScriptableObject, axeReceivedDialogue, AudioManager.Sound.R1_Box_Axe_Received);
            }

            // Delete key from inventory
            InventoryManager.instance.DeleteInventoryItem(lobbyKeyScriptableObject);

            // Update this completed interaction in the GameData class
            GameData.r1_treasureBoxAxeCollected = true;

            // Disable the treasure box's collider
            DisableCollider();
        }
        else if (roomIndex == RoomIndex.Room2)
        {
            // Set treasure box image display to box open sprite
            imageDisplayGO.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.treasureBoxOpenSprite;

            // Show magic potion received on description box and following dialogue
            if (DescriptionBox.instance)
            {
                string[] magicPotionReceivedDialogue = magicPotionScriptableObject.playerComments;
                DescriptionBox.instance.ShowRewardInDescBoxAfterDelay(0.5f, magicPotionScriptableObject, magicPotionReceivedDialogue, AudioManager.Sound.MagicPotionCollect);
            }

            // Delete key from inventory
            InventoryManager.instance.DeleteInventoryItem(room2TreasureBoxKeyScriptableObject);

            // Update this completed interaction in the GameData class
            GameData.r2_treasureBoxMagicPotionCollected = true;

            // Disable the treasure box's collider
            DisableCollider();
        }
    }

    public void EnableCollider()
    {
        boxCollider.enabled = true;
    }

    public void DisableCollider()
    {
        boxCollider.enabled = false;
    }
}
