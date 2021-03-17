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
    private Image itemIcon;
    private TextMeshProUGUI nameText;

    // private bool draggingItem = false;

    public void Awake()
    {
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
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
            nameText.text = pc_Then_Inventory_Object.itemName;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Fade in item name
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, 0f, 1f, 0f, 0.1f));

        // Play hover sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryMouseHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Fade out item name
        new Task(UIAnimation.FadeTMProTextAfterDelay(nameText, nameText.alpha, 0f, 0f, 0.1f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("clicked on item");
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
}
