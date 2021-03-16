using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_PC_Then_Note_Object", menuName = "Inventory System/Items/PC_Then_Note_Object")]
public class PC_Then_Note_Object : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.PC_Then_Note;
    }

    public string sceneName;                        // String - Name of the scene in which the item is present

    [Header("Player Comment")]
    public bool hasPlayerComment;                   // Bool - Object has player comment
    [TextArea(4, 10)]
    public string[] playerComments;                 // String array - Comments of the player on interaction

    [Header("Note Display")]
    public bool hasNote;                            // Object has a note associated with it
    public bool largeNote;                          // Bool - 1 if large note display or 0 for small note display
    [TextArea(4, 10)]
    public string noteText;                         // Note text

    [Header("Response After Note Display")]
    public bool hasResponseAfterNote;               // If player responds after reading the note
    [TextArea(4, 10)]
    public string[] responseTexts;                  // Response text after reading note
}
