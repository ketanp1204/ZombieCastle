using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Private references
    public Image slotSelectedBackground;
    public TextMeshProUGUI nameText;

    // Public variables
    [HideInInspector]
    public PlayerCombat.WeaponTypes weaponType;
    [HideInInspector]
    public WeaponObject scriptableObject;
    [HideInInspector]
    public bool canInteract = true;

    // Private variables
    private bool slotSelected = false;
    private bool weaponWasHighlighted = false;

    // Treasure box interaction
    private bool isTreasureBoxInteraction = false;
    private TreasureBoxInteraction treasureBoxInteractionScript = null;

    public void DisableInteraction()
    {
        canInteract = false;
    }

    public void EnableInteraction()
    {
        canInteract = true;
    }

    public void SetWeaponToSelected()
    {
        if (!slotSelected)
        {
            slotSelected = true;
            Color c = slotSelectedBackground.color;
            c.a = 1f;
            slotSelectedBackground.color = c;
        }
    }

    public void UnselectWeapon()
    {
        if (slotSelected)
        {
            slotSelected = false;
            Color c = slotSelectedBackground.color;
            c.a = 0f;
            slotSelectedBackground.color = c;
        }
    }

    public void Highlight()
    {
        weaponWasHighlighted = true;
        Color c = slotSelectedBackground.color;
        c.a = 0.4f;
        slotSelectedBackground.color = c;
    }

    public void HideSelectedBackground()
    {
        Color c = slotSelectedBackground.color;
        c.a = 0f;
        slotSelectedBackground.color = c;
    }

    public void SetTreasureBoxInteractionFlag()
    {
        isTreasureBoxInteraction = true;
    }

    public void SetTreasureBoxInteractionScriptInstance(TreasureBoxInteraction instance)
    {
        treasureBoxInteractionScript = instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canInteract)
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canInteract)
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canInteract)
        {
            if (isTreasureBoxInteraction && weaponWasHighlighted)
            {
                // Hide inventory box
                InventoryManager.HideInventory();

                // Treasure box interaction behaviour
                treasureBoxInteractionScript.BehaviourAfterInventoryItemSelected();

                // Re-enable interaction of all other disabled slots
                InventoryManager.instance.EnableInteractionForAllWeaponSlots();
                InventoryManager.instance.EnableInteractionForAllItemSlots();

                // Reset treasure box interaction bools
                weaponWasHighlighted = false;
                isTreasureBoxInteraction = false;
            }
            else
            {
                // Close Inventory Box
                InventoryManager.HideInventory();

                // Set/unset slot selected bool and show/hide background
                if (!slotSelected)
                {
                    SetWeaponToSelected();

                    // Unset selected bool of other weapon slots
                    foreach (Transform child in transform.parent)
                    {
                        if (child.gameObject != gameObject)
                        {
                            child.GetComponent<WeaponSlotInteraction>().UnselectWeapon();
                        }
                    }
                }
                else
                {
                    UnselectWeapon();
                }

                EquipSelectedWeapon();
            }
        }
    }

    private void EquipSelectedWeapon()
    {
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
