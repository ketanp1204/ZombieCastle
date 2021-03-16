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

    public Sprite inventorySprite;
    public PlayerCombat.WeaponTypes weaponType;
}
