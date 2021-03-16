using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Note_Only_Object", menuName = "Inventory System/Items/Note_Only_Object")]
public class NoteOnlyObject : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.NoteOnly;
    }

    [Header("Note Display")]
    public bool hasNote;                        // Object has a note associated with it
    public bool largeNote;                      // Bool - 1 if large note display or 0 for small note display
    [TextArea(4, 10)]
    public string noteText;                     // Note text

    [Header("Response After Note Display")]
    public bool hasResponseAfterNote;           // If player responds after reading the note
    [TextArea(4, 10)]
    public string[] responseTexts;              // Response text after reading note
}
