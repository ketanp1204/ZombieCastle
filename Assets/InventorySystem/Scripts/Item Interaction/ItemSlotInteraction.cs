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


    // Private variables
    // private bool draggingItem = false;
    private bool slotSelected = false;
    private bool itemWasHighlighted = false;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canInteract)
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
            if (isTreasureBoxInteraction && itemWasHighlighted)
            {
                // Hide inventory box
                InventoryManager.HideInventory();

                // Treasure box interaction behaviour
                treasureBoxInteractionScript.BehaviourAfterInventoryItemSelected();

                // Re-enable interaction of all other disabled slots
                InventoryManager.instance.EnableInteractionForAllWeaponSlots();
                InventoryManager.instance.EnableInteractionForAllItemSlots();

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
            Debug.Log("dragging item");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canInteract)
        {
            Debug.Log("begin dragging");
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canInteract)
        {
            Debug.Log("end dragging");
        }   
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
