﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_PC_Then_Inventory_Object", menuName = "Inventory System/Items/PC_Then_Inventory_Object")]
public class PC_Then_Inventory_Object : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.PC_Then_Inventory;
    }

    public string sceneName;                                        // String - Name of the scene in which the item is present

    [Header("Player Comment")]
    public bool hasPlayerComment;                                   // Bool - Object has player comment
    [TextArea(4, 10)]
    public string[] playerComments;                                 // String array - Comments of the player on interaction

    [Header("Inventory Configuration")]
    public bool destroyFromSceneAfterAddingToInventory;             // Bool - Used to remove the display of the object from scene after interaction
    public string inventoryItemName;                                // String - Name to be used in the inventory box
    public Sprite inventorySprite;                                  // Sprite - Sprite for the object in the inventory
    public bool canCombineWithAdditionalItem1;                      // Bool - Whether this item can be combined with additionalItem1
    public ItemObject additionalItem1;                              // ItemObject - item#1 that can be combined with this item
    public ItemObject combinedObject1;                              // ItemObject - the result of combination of this item with additionalItem1

    public bool canCombineWithAdditionalItem2;                      // Bool - Whether this item can be combined with additionalItem2
    public ItemObject additionalItem2;                              // ItemObject - item#2 that can be combined with this item
    public ItemObject combinedObject2;                              // ItemObject - the result of combination of this item with additionalItem2
}
