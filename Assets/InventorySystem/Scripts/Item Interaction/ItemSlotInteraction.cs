using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Private Cached References
    public Image slotSelectedBackground;
    public Image itemIcon;
    public TextMeshProUGUI nameText;

    // Public variables
    [HideInInspector]
    public ItemObject itemScriptableObject;
    [HideInInspector]
    public bool canInteract = true;

    // For inventory drag and drop
    [HideInInspector]
    public bool itemOverAdditionalSlot = false;                                         // Bool - Item is hovering over item which can be combined with this item
    [HideInInspector]
    public ItemObject additionalItemScriptableObjectUnderDraggingItem = null;           // ItemObject - ItemObject below this item that can be combined with this item


    // Private variables
    // private bool draggingItem = false;
    private bool slotSelected = false;
    private bool itemWasHighlighted = false;
    private bool thisItemBeingDragged = false;

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

    public void Highlight()
    {
        itemWasHighlighted = true;
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

    public void ResetDraggingAdditionalItemData()
    {
        itemOverAdditionalSlot = false;
        additionalItemScriptableObjectUnderDraggingItem = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryManager.instance.isDraggingItem)
        {
            if (!thisItemBeingDragged)
            {
                if (itemScriptableObject.itemType == ItemType.PC_Then_Inventory)
                {
                    PC_Then_Inventory_Object obj = (PC_Then_Inventory_Object)itemScriptableObject;

                    if (obj.canCombineWithAdditionalItem1)
                    {
                        ItemSlotInteraction script = InventoryManager.instance.draggingItemSlotScript;

                        if (script.itemScriptableObject == obj.additionalItem1)
                        {
                            ShowItemNameAndPlaySoundOnHover();
                            script.itemOverAdditionalSlot = true;
                            script.additionalItemScriptableObjectUnderDraggingItem = itemScriptableObject;
                        }
                        else
                        {
                            if (obj.canCombineWithAdditionalItem2)
                            {
                                if (script == obj.additionalItem2)
                                {
                                    ShowItemNameAndPlaySoundOnHover();
                                    script.itemOverAdditionalSlot = true;
                                    script.additionalItemScriptableObjectUnderDraggingItem = itemScriptableObject;
                                }
                            }
                        }
                    }
                }
                else if (itemScriptableObject.itemType == ItemType.DescBox_Then_Dialogue)
                {
                    DescBox_Then_Dialogue_Object obj = (DescBox_Then_Dialogue_Object)itemScriptableObject;

                    if (obj.canCombineWithAdditionalItem1)
                    {
                        ItemSlotInteraction script = InventoryManager.instance.draggingItemSlotScript;

                        if (script.itemScriptableObject == obj.additionalItem1)
                        {
                            ShowItemNameAndPlaySoundOnHover();
                            script.itemOverAdditionalSlot = true;
                            script.additionalItemScriptableObjectUnderDraggingItem = itemScriptableObject;
                        }
                        else
                        {
                            if (obj.canCombineWithAdditionalItem2)
                            {
                                if (script.itemScriptableObject == obj.additionalItem2)
                                {
                                    ShowItemNameAndPlaySoundOnHover();
                                    script.itemOverAdditionalSlot = true;
                                    script.additionalItemScriptableObjectUnderDraggingItem = itemScriptableObject;
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (InventoryManager.instance.isDraggingWeapon)
        {
            if (itemScriptableObject.itemType == ItemType.PC_Then_Inventory)
            {
                PC_Then_Inventory_Object obj = (PC_Then_Inventory_Object)itemScriptableObject;

                if (obj.canCombineWithAdditionalItem1)
                {
                    WeaponSlotInteraction script = InventoryManager.instance.draggingWeaponSlotScript;

                    if (script.scriptableObject == obj.additionalItem1)
                    {
                        ShowItemNameAndPlaySoundOnHover();
                        script.weaponOverAdditionalSlot = true;
                        script.additionalItemScriptableObjectUnderDraggingWeapon = itemScriptableObject;
                    }
                    else
                    {
                        if (obj.canCombineWithAdditionalItem2)
                        {
                            if (script.scriptableObject == obj.additionalItem2)
                            {
                                ShowItemNameAndPlaySoundOnHover();
                                script.weaponOverAdditionalSlot = true;
                                script.additionalItemScriptableObjectUnderDraggingWeapon = itemScriptableObject;
                            }
                        }
                    }
                }
            }
            else if (itemScriptableObject.itemType == ItemType.DescBox_Then_Dialogue)
            {
                DescBox_Then_Dialogue_Object obj = (DescBox_Then_Dialogue_Object)itemScriptableObject;

                if (obj.canCombineWithAdditionalItem1)
                {
                    WeaponSlotInteraction script = InventoryManager.instance.draggingWeaponSlotScript;

                    if (script.scriptableObject == obj.additionalItem1)
                    {
                        ShowItemNameAndPlaySoundOnHover();
                        script.weaponOverAdditionalSlot = true;
                        script.additionalItemScriptableObjectUnderDraggingWeapon = itemScriptableObject;
                    }
                    else
                    {
                        if (obj.canCombineWithAdditionalItem2)
                        {
                            if (script.scriptableObject == obj.additionalItem2)
                            {
                                ShowItemNameAndPlaySoundOnHover();
                                script.weaponOverAdditionalSlot = true;
                                script.additionalItemScriptableObjectUnderDraggingWeapon = itemScriptableObject;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (canInteract)
            {
                ShowItemNameAndPlaySoundOnHover();
            }
        }
    }

    private void ShowItemNameAndPlaySoundOnHover()
    {
        // Fade in item name
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.1f));

        // Play hover sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryMouseHover);

        // Partially fade in slot selected background if not selected
        if (!slotSelected)
        {
            Color c = slotSelectedBackground.color;
            c.a = 0.4f;
            slotSelectedBackground.color = c;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventoryManager.instance.isDraggingItem)
        {
            if (!thisItemBeingDragged)
            {
                InventoryManager.instance.draggingItemSlotScript.ResetDraggingAdditionalItemData();

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
        else if (InventoryManager.instance.isDraggingWeapon)
        {
            InventoryManager.instance.draggingWeaponSlotScript.ResetDraggingAdditionalItemData();

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
        else
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
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!InventoryManager.instance.isDraggingItem && !InventoryManager.instance.isDraggingWeapon)
        {
            if (canInteract)
            {
                if (isTreasureBoxInteraction && itemWasHighlighted)
                {
                    // Hide inventory box
                    InventoryManager.HideInventory();

                    // Treasure box interaction behaviour
                    treasureBoxInteractionScript.BehaviourAfterInventoryItemSelected();

                    // Re-enable interaction of all other disabled slots
                    InventoryManager.instance.EnableInteractionOfAllSlots();

                    // Reset treasure box interaction bools
                    itemWasHighlighted = false;
                    isTreasureBoxInteraction = false;
                }
                else
                {
                    // Set slot selected bool and show background
                    if (!slotSelected)
                    {
                        slotSelected = true;
                        Color c = slotSelectedBackground.color;
                        c.a = 1f;
                        slotSelectedBackground.color = c;

                        new Task(UnselectSlotAfterDelay(0.5f));
                    }

                    if (DescriptionBox.instance)
                    {
                        DescriptionBox.instance.ShowDescBoxFromInventory(itemScriptableObject);
                    }
                }
            }
        }
    }

    private IEnumerator UnselectSlotAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        slotSelected = false; 
        Color c = slotSelectedBackground.color;
        c.a = 0f;
        slotSelectedBackground.color = c;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canInteract)
        {
            if (InventoryManager.instance.isDraggingItem)
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
            if (itemScriptableObject.itemType == ItemType.PC_Then_Inventory)
            {
                PC_Then_Inventory_Object obj = (PC_Then_Inventory_Object)itemScriptableObject;

                if (obj.canCombineWithAdditionalItem1)
                {
                    // If the future, remove these hard coded dependencies
                    if (obj.additionalItem1.itemType != ItemType.Weapon && obj.inventoryItemName != "Oil")
                    {
                        itemIsDraggable = true;
                    }
                }
            }
            else if (itemScriptableObject.itemType == ItemType.DescBox_Then_Dialogue)
            {
                DescBox_Then_Dialogue_Object obj = (DescBox_Then_Dialogue_Object)itemScriptableObject;

                if (obj.canCombineWithAdditionalItem1)
                {
                    if (obj.additionalItem1.itemType != ItemType.Weapon)
                    {
                        itemIsDraggable = true;
                    }
                }
            }

            if (itemIsDraggable)
            {
                thisItemBeingDragged = true;
                InventoryManager.instance.isDraggingItem = true;
                InventoryManager.instance.draggingItemSlotScript = this;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!itemOverAdditionalSlot)
        {
            // Nothing under the dragging item, reset inventory dragging bool and script and restore dragging item icon position to its own slot
            StopItemDragAndReset();
        }
        else
        {
            // Can combine with either of the two additionalItems of this item
            if (additionalItemScriptableObjectUnderDraggingItem)
            {
                if (itemScriptableObject.itemType == ItemType.PC_Then_Inventory)
                {
                    PC_Then_Inventory_Object obj = (PC_Then_Inventory_Object)itemScriptableObject;

                    if (additionalItemScriptableObjectUnderDraggingItem == obj.additionalItem1)
                    {
                        // In the future remove this hard coded dependency
                        string text = "Torch combined with Oil";
                        InventoryManager.instance.ShowTextOnHighlightText(text, 0f, 1f);

                        // Play inventory drag and drop sound
                        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.InventoryDragAndDrop);

                        StopItemDragAndReset();

                        // Remove this item and additionalItem1 from inventory
                        InventoryManager.instance.DeleteInventoryItem(obj);
                        InventoryManager.instance.DeleteInventoryItem(obj.additionalItem1);

                        // Add combinedObject1 to inventory
                        InventoryManager.instance.AddInventoryItem(obj.combinedObject1);
                    }
                    else if (additionalItemScriptableObjectUnderDraggingItem == obj.additionalItem2)
                    {
                        // In the future add these interactions if required
                        string text = "";
                        InventoryManager.instance.ShowTextOnHighlightText(text, 0f, 1f);

                        StopItemDragAndReset();

                        // Remove this item and additionalItem2 from inventory
                        InventoryManager.instance.DeleteInventoryItem(obj);
                        InventoryManager.instance.DeleteInventoryItem(obj.additionalItem2);

                        // Add combinedObject2 to inventory
                        GameData.currentPlayerInventory.AddItem(obj.combinedObject2, 1);

                        // Update inventory
                        InventoryManager.instance.AddInventoryItem(obj.combinedObject2);
                    }
                }
                else if (itemScriptableObject.itemType == ItemType.DescBox_Then_Dialogue)
                {
                    DescBox_Then_Dialogue_Object obj = (DescBox_Then_Dialogue_Object)itemScriptableObject;

                    if (additionalItemScriptableObjectUnderDraggingItem == obj.additionalItem1)
                    {
                        Debug.Log("item combined with additionalItem1");
                        StopItemDragAndReset();

                        // Remove this item and additionalItem1 from inventory
                        GameData.currentPlayerInventory.RemoveItem(obj);
                        GameData.currentPlayerInventory.RemoveItem(obj.additionalItem1);

                        // Add combinedObject1 to inventory
                        GameData.currentPlayerInventory.AddItem(obj.combinedObject1, 1);

                        // Update inventory
                        InventoryManager.instance.UpdateInventorySlots();
                    }
                    else if (additionalItemScriptableObjectUnderDraggingItem == obj.additionalItem2)
                    {
                        Debug.Log("item combined with additionalItem2");
                        StopItemDragAndReset();

                        // Remove this item and additionalItem2 from inventory
                        GameData.currentPlayerInventory.RemoveItem(obj);
                        GameData.currentPlayerInventory.RemoveItem(obj.additionalItem2);

                        // Add combinedObject2 to inventory
                        GameData.currentPlayerInventory.AddItem(obj.combinedObject2, 1);

                        // Update inventory
                        InventoryManager.instance.UpdateInventorySlots();
                    }
                }
            }
        }
    }

    private void StopItemDragAndReset()
    {
        thisItemBeingDragged = false;
        InventoryManager.instance.isDraggingItem = false;
        InventoryManager.instance.draggingItemSlotScript = null;
        itemIcon.gameObject.transform.localPosition = Vector3.zero;
    }

    public void PopulateItemSlot(ItemObject scriptableObject)
    {
        if (scriptableObject.itemType == ItemType.PC_Then_Inventory)
        {
            PC_Then_Inventory_Object pc_Then_Inventory_Object = (PC_Then_Inventory_Object)scriptableObject;

            itemIcon.sprite = pc_Then_Inventory_Object.inventorySprite;

            // Set ItemIcon image component's alpha to 1
            Color c = itemIcon.color;
            c.a = 1f;
            itemIcon.color = c;

            // Set NameText 
            nameText.text = pc_Then_Inventory_Object.inventoryItemName;

            // Set itemScriptableObject
            itemScriptableObject = scriptableObject;
        }
        else if (scriptableObject.itemType == ItemType.DescBox_Then_Dialogue)
        {
            DescBox_Then_Dialogue_Object descBox_Then_Dialogue_Object = (DescBox_Then_Dialogue_Object)scriptableObject;

            itemIcon.sprite = descBox_Then_Dialogue_Object.inventorySprite;

            // Set ItemIcon image component's alpha to 1
            Color c = itemIcon.color;
            c.a = 1f;
            itemIcon.color = c;

            // Set NameText 
            nameText.text = descBox_Then_Dialogue_Object.inventoryItemName;

            // Set itemScriptableObject
            itemScriptableObject = scriptableObject;
        }
    }
}
