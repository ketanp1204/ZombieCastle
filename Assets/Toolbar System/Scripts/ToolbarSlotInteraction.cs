using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ToolbarSlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public enum ToolbarSlotType
    {
        Inventory,
        Settings
    }

    // Public variables
    public ToolbarSlotType toolbarSlotType;

    // Private references
    private UIReferences uiReferences;
    private TextMeshProUGUI nameText;
    

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
    }

    private void SetReferences()
    {
        uiReferences = GameSession.instance.uiReferences;
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
        // Hide toolbar
        ToolbarManager.instance.HideToolbar();

        if (toolbarSlotType == ToolbarSlotType.Inventory)
        {
            // Open inventory
            InventoryManager.ShowInventory();
        }
        else if(toolbarSlotType == ToolbarSlotType.Settings)
        {
            // Play paper pickup sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PaperPickup);

            if (SettingsMenu.instance)
            {
                SettingsMenu.instance.ShowSettingsMenu();
            }
        }
    }
}
