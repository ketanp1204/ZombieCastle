using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector]
    public ItemObject itemScriptableObject;

    // Private Cached References
    public Image slotSelectedBackground;
    public Image itemIcon;
    public TextMeshProUGUI nameText;

    // Private variables
    // private bool draggingItem = false;
    private bool slotSelected = false;

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
            nameText.text = pc_Then_Inventory_Object.itemName;

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

    public void OnPointerEnter(PointerEventData eventData)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        // Set slot selected bool and show background
        if (!slotSelected)
        {
            slotSelected = true;
            Color c = slotSelectedBackground.color;
            c.a = 1f;
            slotSelectedBackground.color = c;
        }

        if (DescriptionBox.instance)
        {
            DescriptionBox.instance.ShowDescBoxFromInventory(itemScriptableObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("dragging item");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("end dragging");
    }

    void Update()
    {
        if (slotSelected)
        {
            if (!DescriptionBox.instance.isActive)
            {
                DeselectSlot();
            }
        }
    }

    private void DeselectSlot()
    {
        if (slotSelected)
        {
            slotSelected = false;
            Color c = slotSelectedBackground.color;
            c.a = 0f;
            slotSelectedBackground.color = c;
        }
    }
}
