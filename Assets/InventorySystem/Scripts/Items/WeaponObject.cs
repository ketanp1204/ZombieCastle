using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory System/Items/Weapon")]
public class WeaponObject : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.Weapon;
    }

    public PlayerCombat.WeaponTypes weaponType;

    [Header("Inventory Configuration")]
    public Sprite inventorySprite;
    public bool canCombineWithAdditionalItem1;                      // Bool - Whether this item can be combined with additionalItem1
    public ItemObject additionalItem1;                              // ItemObject - item#1 that can be combined with this item
    public ItemObject combinedObject1;                              // ItemObject - the result of combination of this item with additionalItem1

    public bool canCombineWithAdditionalItem2;                      // Bool - Whether this item can be combined with additionalItem2
    public ItemObject additionalItem2;                              // ItemObject - item#2 that can be combined with this item
    public ItemObject combinedObject2;                              // ItemObject - the result of combination of this item with additionalItem2
}
