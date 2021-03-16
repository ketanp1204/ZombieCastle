using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Types of interactable objects are defined by this enum. These include:
/// 
///     1. Weapon - Weapons received in game through puzzle completion, treasure box, etc
///     2. PC_Only - Objects that have a player comment dialogue only
///     3. PC_Then_Inventory - Objects that have a player comment dialogue followed by addition of this item to the Inventory
///     4. PC_Then_Note - Objects that have a player comment dialogue followed by a note display
///     5. NoteOnly - Objects that have a note box display only
///     6. Default - Default Item class
/// 
/// </summary>
public enum ItemType
{
    Weapon,
    PC_Only,
    PC_Then_Inventory,
    PC_Then_Note,
    NoteOnly,
    Default
}

public abstract class ItemObject : ScriptableObject
{
    public string itemName;
    [TextArea(6, 10)]
    public string itemDescription;      // Description of the item

    public ItemType itemType;
}
