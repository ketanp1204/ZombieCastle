using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultObject", menuName = "Inventory System/Items/Default Item")]
public class DefaultItem : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.Default;
    }

    public string sceneName;            // Name of the scene in which the item is present

    [TextArea(4, 10)]
    public string playerComment;        // Comment of the player on seeing this item

    [Header("Note Display")]
    public bool hasNote;                // Object has a note associated with it
    public string noteText;             // Note text

    [Header("Response After Note Display")]
    public bool hasResponseAfterNote;   // If player responds after reading the note
    public string responseText;         // Response text after reading note

    [Header("Book Display")]
    public bool hasBookDisplay;         // Object has a book display associated with it
    [Header("For each textbox, character limit is 344")]
    [TextArea(4, 7)]
    public string[] bookTexts;          // Book texts     
}
