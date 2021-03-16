using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCommentOnlyObject", menuName = "Inventory System/Items/PlayerCommentOnlyObject")]
public class PlayerCommentOnlyObject : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.PC_Only;
    }

    public string sceneName;                // String - Name of the scene in which the item is present

    [Header("Player Comment")]
    public bool hasPlayerComment;           // Bool - Object has player comment
    [TextArea(4, 10)]
    public string[] playerComments;         // String array - Comments of the player on interaction
}
