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
    public bool hasNote;                    // Object has a note associated with it
    public string noteText;                 // Note text

    [Header("Response After Note Display")]
    public bool hasResponseAfterNote;       // If player responds after reading the note
    public string responseText;             // Response text after reading note
}
