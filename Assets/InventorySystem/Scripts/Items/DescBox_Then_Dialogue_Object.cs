using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DescBoxThenDialogueObject", menuName = "Inventory System/Items/DescBoxThenDialogueObject")]
public class DescBox_Then_Dialogue_Object : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.DescBox_Then_Dialogue;
    }

    public string sceneName;                // String - Name of the scene in which the item is present

    public string inventoryItemName;        // String - Name of the object to be displayed in the inventory box

    [Header("Player Comment")]
    public bool hasPlayerComment;           // Bool - Object has player comment
    [TextArea(4, 10)]
    public string[] playerComments;         // String array - Comments of the player on interaction

    public Sprite inventorySprite;          // Sprite - Sprite for the object in the inventory
}
