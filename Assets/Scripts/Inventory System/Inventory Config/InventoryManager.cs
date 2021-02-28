using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // Public Cached References
    public InventoryObject inventory;
    public GameObject inventorySlotPrefab;

    // Private Cached References
    private List<GameObject> inventorySlots = new List<GameObject>();
    private CanvasGroup inventoryCanvasGroup;
    private PlayerInput playerInput;

    // Private variables
    private bool isInventoryOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        inventoryCanvasGroup = GetComponent<CanvasGroup>();
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!isInventoryOpen)
                ShowInventory();
            else if (isInventoryOpen)
                HideInventory();
        }
    }

    private void ShowInventory()
    {
        isInventoryOpen = true;

        // Show inventory display
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(inventoryCanvasGroup, 0f, 1f, 0f, 0.1f));
        // Enable mouse interaction
        inventoryCanvasGroup.interactable = true;
        inventoryCanvasGroup.blocksRaycasts = true;

        // Enable mouse cursor
        Cursor.visible = true;

        // Stop player movement
        if (Player.instance)
        {
            Player.StopMovement();
        }
        if (PlayerTopDown.instance)
        {
            PlayerTopDown.StopMovement();
        }
        
    }

    private void HideInventory()
    {
        isInventoryOpen = false;

        // Hide inventory display
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(inventoryCanvasGroup, inventoryCanvasGroup.alpha, 0f, 0f, 0.1f));
        // Prevent mouse interaction
        inventoryCanvasGroup.interactable = false;
        inventoryCanvasGroup.blocksRaycasts = false;

        // Hide mouse cursor
        Cursor.visible = false;

        // Resume player movement
        if (Player.instance)
        {
            Player.EnableMovement();
        }
        if (PlayerTopDown.instance)
        {
            PlayerTopDown.EnableMovement();
        }
    }

    private void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            // Instantiate inventory display slot
            GameObject slot = Instantiate(inventorySlotPrefab, transform.GetChild(0));
            inventorySlots.Add(slot);

            // Get Graphic Image component
            Image graphic = slot.GetComponentInChildren<Image>();

            // Get item sprite from ScriptableObject data
            graphic.sprite = inventory.Container[i].item.inventorySprite;

            // Set the display image component's alpha to 1
            Color c = graphic.color;
            c.a = 1f;
            graphic.color = c;

            // Get AmountText TextMeshProUGUI component
            TextMeshProUGUI amountText = slot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();

            // Set AmountText text if quantity is greater than 1
            if (inventory.Container[i].amount > 1)
            {
                amountText.text = inventory.Container[i].amount.ToString();
            }

            // Get NameText TextMeshProUGUI component
            TextMeshProUGUI nameText = slot.transform.Find("NameText").GetComponent<TextMeshProUGUI>();

            // Set NameText text
            nameText.text = inventory.Container[i].item.itemName;
        }
    }
}
