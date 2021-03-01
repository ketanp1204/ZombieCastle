using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Default
}

public abstract class ItemObject : ScriptableObject
{
    public Sprite inventorySprite;
    public string itemName;
    [TextArea(6, 10)]
    public string itemDescription;      // Description of the item

    public ItemType itemType;
}
