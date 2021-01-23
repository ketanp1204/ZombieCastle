using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Simple Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string itemDescription;      // Description of the item
    [TextArea(4, 10)]
    public string playerComment;        // Comment of the player on seeing this item

    public Sprite itemSprite;
    public string sceneName;            // Name of the scene in which the item is present
    public bool hasNote;                // Object has a note associated with it
    public string noteText;             // Note text

    public bool hasResponseAfterNote;   // If player responds after reading the note
    public string responseText;         // Response text after reading note

    public bool hasBookDisplay;         // Object has a book display associated with it
    [Header("For each textbox, character limit is 344")]
    [TextArea(4, 7)]
    public string[] bookTexts;          // Book texts             

}
