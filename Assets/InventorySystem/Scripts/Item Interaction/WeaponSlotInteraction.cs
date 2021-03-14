using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponSlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Private references
    private TextMeshProUGUI nameText;

    // Public variables
    [HideInInspector]
    public PlayerCombat.WeaponTypes weaponType;

    private void Start()
    {
        nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Fade in item name
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Fade out item name
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, nameText.alpha, 0f, 0f, 0.1f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Close Inventory Box
        InventoryManager.HideInventory();

        // Check whether weapon is equipped
        if (weaponType == PlayerCombat.WeaponTypes.Knife)
        {
            if (Player.instance != null)
            {
                if (Player.KnifeDrawn())
                {
                    // Unequip
                    Player.UnequipKnife();
                }
                else
                {
                    // Equip
                    Player.EquipKnife();
                }
            }
            else
            {
                if (PlayerTopDown.KnifeDrawn())
                {
                    // Unequip
                    PlayerTopDown.UnequipKnife();
                }
                else
                {
                    // Equip
                    PlayerTopDown.EquipKnife();
                }
            }
        }
        else if (weaponType == PlayerCombat.WeaponTypes.Axe)
        {
            if (Player.instance != null)
            {
                if (Player.AxeDrawn())
                {
                    // Unequip
                    Player.UnequipAxe();
                }
                else
                {
                    // Equip
                    Player.EquipAxe();
                }
            }
            else
            {
                if (PlayerTopDown.AxeDrawn())
                {
                    // Unequip
                    PlayerTopDown.UnequipAxe();
                }
                else
                {
                    // Equip
                    PlayerTopDown.EquipAxe();
                }
            }
        }
    }

}
