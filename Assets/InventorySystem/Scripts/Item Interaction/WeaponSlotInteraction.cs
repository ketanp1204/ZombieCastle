using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Private references
    public Image slotSelectedBackground;
    public TextMeshProUGUI nameText;

    // Public variables
    [HideInInspector]
    public PlayerCombat.WeaponTypes weaponType;

    // Private variables
    private bool slotSelected = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Fade in item name
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.1f));

        // Play hover sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryMouseHover);

        // Partially fade in slot selected background
        if (!slotSelected)
        {
            Color c = slotSelectedBackground.color;
            c.a = 0.4f;
            slotSelectedBackground.color = c;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Fade out item name
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, nameText.alpha, 0f, 0f, 0.1f));

        // Hide slot selected background if not selected
        if (!slotSelected)
        {
            Color c = slotSelectedBackground.color;
            c.a = 0f;
            slotSelectedBackground.color = c;
        }
    }

    public void SetSlotSelected()
    {
        if (!slotSelected)
        {
            slotSelected = true;
            Color c = slotSelectedBackground.color;
            c.a = 1f;
            slotSelectedBackground.color = c;
        }
    }

    public void UnsetSlotSelected()
    {
        if (slotSelected)
        {
            slotSelected = false;
            Color c = slotSelectedBackground.color;
            c.a = 0f;
            slotSelectedBackground.color = c;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Close Inventory Box
        InventoryManager.HideInventory();

        // Set/unset slot selected bool and show/hide background
        if (!slotSelected)
        {
            SetSlotSelected();

            // Unset selected bool of other weapon slots
            foreach (Transform child in transform.parent)
            {
                if (child.gameObject != gameObject)
                {
                    child.GetComponent<WeaponSlotInteraction>().UnsetSlotSelected();
                }
            }
        }
        else
        {
            UnsetSlotSelected();
        }

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
