using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_PC_Then_Inventory_Object", menuName = "Inventory System/Items/PC_Then_Inventory_Object")]
public class PC_Then_Inventory_Object : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.PC_Then_Inventory;
    }

    public string sceneName;                // String - Name of the scene in which the item is present

    [Header("Player Comment")]
    public bool hasPlayerComment;           // Bool - Object has player comment
    [TextArea(4, 10)]
    public string[] playerComments;         // String array - Comments of the player on interaction

    public Sprite inventorySprite;          // Sprite - Sprite for the object in the inventory
}
