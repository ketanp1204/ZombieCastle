using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Private references
    public Image slotSelectedBackground;
    public Image itemIcon;
    public TextMeshProUGUI nameText;

    // Public variables
    [HideInInspector]
    public PlayerCombat.WeaponTypes weaponType;
    [HideInInspector]
    public WeaponObject scriptableObject;
    [HideInInspector]
    public bool canInteract = true;

    // For inventory drag and drop
    [HideInInspector]
    public bool weaponOverAdditionalSlot = false;                                       // Bool - Weapon is hovering over item which can be combined with this weapon
    [HideInInspector]
    public ItemObject additionalItemScriptableObjectUnderDraggingWeapon = null;         // ItemObject - ItemObject below this weapon that can be combined with this weapon

    // Private variables
    private bool slotSelected = false;
    private bool weaponWasHighlighted = false;

    // To highlight weapon before combat starts in room 1
    private bool highlightWeaponBeforeCombatStartFlag = false;

    // Treasure box interaction
    private bool isTreasureBoxInteraction = false;
    private TreasureBoxInteraction treasureBoxInteractionScript = null;

    /// <summary>
    /// Slot interaction control
    /// </summary>


    public void DisableInteraction()
    {
        canInteract = false;

        // Hide selected background
        HideSelectedBackground();

        // Partially show image icon
        PartiallyShowImageIcon();
    }

    public void EnableInteraction()
    {
        canInteract = true;

        FullyShowImageIcon();
    }

    /// <summary>
    /// UI Utilities
    /// </summary>
    public void Highlight()
    {
        weaponWasHighlighted = true;
        PartiallyShowSelectedBackground();
    }

    public void HideSelectedBackground()
    {
        Color c = slotSelectedBackground.color;
        c.a = 0f;
        slotSelectedBackground.color = c;
    }

    public void PartiallyShowSelectedBackground()
    {
        Color c = slotSelectedBackground.color;
        c.a = 0.4f;
        slotSelectedBackground.color = c;
    }

    public void FullyShowSelectedBackground()
    {
        Color c = slotSelectedBackground.color;
        c.a = 1f;
        slotSelectedBackground.color = c;
    }

    public void PartiallyShowImageIcon()
    {
        Color c = itemIcon.color;
        c.a = 0.3f;
        itemIcon.color = c;
    }

    public void FullyShowImageIcon()
    {
        Color c = itemIcon.color;
        c.a = 1f;
        itemIcon.color = c;
    }

    
    /// <summary>
    /// Special interaction behaviour
    /// </summary>

    public void SetTreasureBoxInteractionFlag()
    {
        isTreasureBoxInteraction = true;
    }

    public void SetTreasureBoxInteractionScriptInstance(TreasureBoxInteraction instance)
    {
        treasureBoxInteractionScript = instance;
    }

    public void SetWeaponHighlightBeforeCombatStartFlag()
    {
        highlightWeaponBeforeCombatStartFlag = true;
    }

    public void ResetDraggingAdditionalItemData()
    {
        weaponOverAdditionalSlot = false;
        additionalItemScriptableObjectUnderDraggingWeapon = null;
    }


    /// <summary>
    /// Mouse interaction of weapon slot
    /// </summary>

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InventoryManager.instance.isDraggingItem && !InventoryManager.instance.isDraggingWeapon)
        {
            if (canInteract)
            {
                // Fade in item name
                new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.1f));

                // Play hover sound
                AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryMouseHover);

                // Partially show slot selected background
                if (!slotSelected)
                {
                    PartiallyShowSelectedBackground();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!InventoryManager.instance.isDraggingWeapon && !InventoryManager.instance.isDraggingWeapon)
        {
            if (canInteract)
            {
                // Fade out item name
                new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, nameText.alpha, 0f, 0f, 0.1f));

                // Hide slot selected background if not selected
                if (!slotSelected)
                {
                    HideSelectedBackground();
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!InventoryManager.instance.isDraggingWeapon && !InventoryManager.instance.isDraggingItem)
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
                    InventoryManager.instance.EnableInteractionOfAllSlots();

                    // Reset bools
                    weaponWasHighlighted = false;
                    isTreasureBoxInteraction = false;
                }
                else if (highlightWeaponBeforeCombatStartFlag && weaponWasHighlighted)
                {
                    // Hide inventory box and enable interaction of all elements
                    InventoryManager.HideInventory();

                    // Re-enable interaction of all other disabled slots
                    InventoryManager.instance.EnableInteractionOfAllSlots();

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

                    // Equip the weapon
                    EquipSelectedWeapon();

                    // Reset bools
                    weaponWasHighlighted = false;
                    highlightWeaponBeforeCombatStartFlag = false;
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

                    // Equip the weapon
                    EquipSelectedWeapon();
                }
            }
        }
    }


    /// <summary>
    /// Drag and drop behaviour
    /// </summary>

    public void OnDrag(PointerEventData eventData)
    {
        if (canInteract)
        {
            if (InventoryManager.instance.isDraggingWeapon)
            {
                itemIcon.gameObject.transform.position = Input.mousePosition;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canInteract)
        {
            bool itemIsDraggable = false;

            // Check if item can be combined with another
            if (scriptableObject.canCombineWithAdditionalItem1)
            {
                itemIsDraggable = true;
            }

            if (itemIsDraggable)
            {
                InventoryManager.instance.isDraggingWeapon = true;
                InventoryManager.instance.draggingWeaponSlotScript = this;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!weaponOverAdditionalSlot)
        {
            // Nothing under the dragging weapon, reset inventory dragging bool and script and restore dragging item icon position to its own slot
            StopWeaponDragAndReset();
        }
        else
        {
            // In the future, remove these hard coded dependencies
            if (additionalItemScriptableObjectUnderDraggingWeapon != null)
            {
                if (additionalItemScriptableObjectUnderDraggingWeapon == scriptableObject.additionalItem1)
                {
                    StopWeaponDragAndReset();

                    // TODO: for sword, enable magic potion combat
                    if (scriptableObject.additionalItem1.itemName == "Magic Potion")
                    {
                        if (PlayerCombat.instance.swordAttackType != PlayerCombat.SwordAttackTypes.Magic)
                        {
                            // Show text on highlight text
                            string text = scriptableObject.itemName + " combined with " + "Elixirium";
                            InventoryManager.instance.ShowTextOnHighlightText(text, 0f, 1f);

                            // Play magic potion sound
                            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.MagicPotionCollect);

                            // Set player combat magic potion status
                            PlayerCombat.instance.swordAttackType = PlayerCombat.SwordAttackTypes.Magic;

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

                            if (!Player.SwordDrawn())
                            {
                                // Equip
                                Player.EquipSword();
                            }
                        }
                        else
                        {
                            // Show normal sword text on highlight text
                            string text = "Normal sword equipped";
                            InventoryManager.instance.ShowTextOnHighlightText(text, 0f, 1f);

                            // Reset to normal sword
                            PlayerCombat.instance.swordAttackType = PlayerCombat.SwordAttackTypes.Normal;
                        }
                    }
                }
                else if (additionalItemScriptableObjectUnderDraggingWeapon == scriptableObject.additionalItem2)
                {
                    StopWeaponDragAndReset();

                    if (scriptableObject.additionalItem2.itemName == "The Eternal Fire")
                    {
                        if (PlayerCombat.instance.swordAttackType != PlayerCombat.SwordAttackTypes.Fire)
                        {
                            // Show text on highlight text
                            string text = scriptableObject.itemName + " combined with " + "The Eternal Fire";
                            InventoryManager.instance.ShowTextOnHighlightText(text, 0f, 1f);

                            // Play fire element sound
                            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.FireElement);

                            // Set player combat magic potion status
                            PlayerCombat.instance.swordAttackType = PlayerCombat.SwordAttackTypes.Fire;

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

                            // Equip the sword
                            if (!Player.SwordDrawn())
                            {
                                // Equip
                                Player.EquipSword();
                            }
                        }
                        else
                        {
                            // Show normal sword text on highlight text
                            string text = "Normal sword equipped";
                            InventoryManager.instance.ShowTextOnHighlightText(text, 0f, 1f);

                            // Reset to normal sword
                            PlayerCombat.instance.swordAttackType = PlayerCombat.SwordAttackTypes.Normal;
                        }
                    }
                }
            }    
        }
    }

    private void StopWeaponDragAndReset()
    {
        InventoryManager.instance.isDraggingWeapon = false;
        InventoryManager.instance.draggingWeaponSlotScript = null;
        itemIcon.gameObject.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Weapon equip/unequip
    /// </summary>

    private void EquipSelectedWeapon()
    {
        // Check whether weapon is equipped
        if (weaponType == PlayerCombat.WeaponTypes.Knife)
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
        else if (weaponType == PlayerCombat.WeaponTypes.Axe)
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
        else if (weaponType == PlayerCombat.WeaponTypes.Sword)
        {
            if (Player.SwordDrawn())
            {
                // Unequip
                Player.UnequipSword();
            }
            else
            {
                // Equip
                Player.EquipSword();
            }
        }
    }

    public void SetWeaponToSelected()
    {
        if (!slotSelected)
        {
            slotSelected = true;
            FullyShowSelectedBackground();
        }
    }

    public void UnselectWeapon()
    {
        if (slotSelected)
        {
            slotSelected = false;
            HideSelectedBackground();
        }
    }
}
