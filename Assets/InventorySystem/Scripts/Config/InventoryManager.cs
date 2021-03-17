using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // Instance
    public static InventoryManager instance;

    // Public Cached References
    public GameObject weaponGridSlotPrefab;
    public GameObject itemGridSlotPrefab;

    // Private Cached References
    private InventoryObject inventory;
    private List<GameObject> weaponSlots = new List<GameObject>();
    private List<GameObject> itemSlots = new List<GameObject>();
    private CanvasGroup inventoryCanvasGroup;

    private Transform weaponGridContainer;
    private Transform itemGridContainer;

    // Private variables
    private bool isInventoryOpen = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();

        // Get inventory scriptable object from player
        if (Player.instance)
        {
            LoadInventory(Player.GetInventory());
        }
        else if(PlayerTopDown.instance)
        {
            LoadInventory(PlayerTopDown.GetInventory());
        }

        // Hide inventory on start
        inventoryCanvasGroup.alpha = 0f;
        inventoryCanvasGroup.interactable = false;
        inventoryCanvasGroup.blocksRaycasts = false;

        // Fill default inventory slots
        FillInventorySlots();
    }

    private void SetReferences()
    {
        weaponGridContainer = transform.GetChild(0).Find("WeaponGrid");
        itemGridContainer = transform.GetChild(0).Find("ItemsGrid");
        inventoryCanvasGroup = GetComponent<CanvasGroup>();
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

        /* NOTE:    In the Unity editor, the close inventory box function does not work as intended. 
         *          On clicking on the close button in the inventory box, the mouse cursor disappears as expected.
         *          But on pressing escape from inventory box to close the inventory, the mouse cursor does not disappear.
         *          However, in the game build, this function works as expected when pressing escape to close the box.
        */

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInventoryOpen)
            {
                HideInventory();
            }
        }
    }

    private void LoadInventory(InventoryObject _inventory)
    {
        inventory = _inventory;
    }

    public static void ShowInventory()
    {
        if (!instance.isInventoryOpen)
        {
            Time.timeScale = 0f;

            instance.isInventoryOpen = true;

            // Play inventory open sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryOpen);

            // Show inventory display
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(instance.inventoryCanvasGroup, 0f, 1f, 0f, 0.3f));
            // Enable mouse interaction
            instance.inventoryCanvasGroup.interactable = true;
            instance.inventoryCanvasGroup.blocksRaycasts = true;

            // Unlock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;

            // Stop player movement
            if (Player.instance)
            {
                Player.StopMovement();
            }
            else if (PlayerTopDown.instance)
            {
                PlayerTopDown.StopMovement();
            }

            // Disable opening pause menu on pressing escape key
            PauseMenu.instance.CanPauseGame = false;
        }
    }

    public static void HideInventory()
    {
        if (instance.isInventoryOpen)
        {
            Time.timeScale = 1f;

            instance.isInventoryOpen = false;

            // Play inventory close sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryClose);

            // Hide inventory display
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(instance.inventoryCanvasGroup, 1f, 0f, 0f, 0.3f));
            // Prevent mouse interaction
            instance.inventoryCanvasGroup.interactable = false;
            instance.inventoryCanvasGroup.blocksRaycasts = false;

            // Lock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;

            // Resume player movement
            if (Player.instance)
            {
                Player.EnableMovement();
            }
            else if (PlayerTopDown.instance)
            {
                PlayerTopDown.EnableMovement();
            }

            // Enable opening pause menu on pressing escape key
            instance.StartCoroutine(PauseMenu.EnableCanPauseGameBoolAfterDelay(0.1f));
        }
    }

    public void AddInventoryItem(ItemObject item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            // Cast item to WeaponObject
            WeaponObject weaponToAdd = (WeaponObject)item;

            // Instantiate weapon grid slot
            GameObject weaponSlot = Instantiate(weaponGridSlotPrefab, weaponGridContainer);

            // Add to local weapon slots list
            weaponSlots.Add(weaponSlot);

            // Get WeaponSlotInteraction script component
            WeaponSlotInteraction interactionScript = weaponSlot.GetComponent<WeaponSlotInteraction>();

            // Set the weapon type on the interaction script
            interactionScript.weaponType = weaponToAdd.weaponType;

            // Get ItemIcon image component
            Image weaponIcon = weaponSlot.transform.Find("ItemIcon").GetComponent<Image>();

            // Set ItemIcon image sprite from ScriptableObject
            weaponIcon.sprite = weaponToAdd.inventorySprite;

            // Set ItemIcon image component's alpha to 1
            Color c = weaponIcon.color;
            c.a = 1f;
            weaponIcon.color = c;

            // Get AmountText TextMeshProUGUI component
            TextMeshProUGUI amountText = weaponSlot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();

            // Get NameText TextMeshProUGUI component
            TextMeshProUGUI nameText = weaponSlot.transform.Find("NameText").GetComponent<TextMeshProUGUI>();

            // Set NameText 
            nameText.text = weaponToAdd.itemName;
        }
        else
        {
            // Instantiate item grid slot
            GameObject itemSlot = Instantiate(itemGridSlotPrefab, itemGridContainer);

            // Add to local item slots list
            itemSlots.Add(itemSlot);

            // Populate slot with scriptable object data
            ItemSlotInteraction itemSlotInteraction = itemSlot.GetComponent<ItemSlotInteraction>();
            itemSlotInteraction.PopulateItemSlot(item);
        }
    }

    private void FillInventorySlots()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            // Check type of item : Add to weapon grid if weapon otherwise into item grid
            if (inventory.Container[i].item.itemType == ItemType.Weapon)
            {
                // Cast item to WeaponObject
                WeaponObject weaponItem = (WeaponObject)inventory.Container[i].item;

                // Instantiate weapon grid slot
                GameObject weaponSlot = Instantiate(weaponGridSlotPrefab, weaponGridContainer);

                // Add to local weapon slots list
                weaponSlots.Add(weaponSlot);

                // Get WeaponSlotInteraction script component
                WeaponSlotInteraction interactionScript = weaponSlot.GetComponent<WeaponSlotInteraction>();

                // Set the weapon type on the interaction script
                interactionScript.weaponType = weaponItem.weaponType;

                // Get ItemIcon image component
                Image weaponIcon = weaponSlot.transform.Find("ItemIcon").GetComponent<Image>();

                // Set ItemIcon image sprite from ScriptableObject
                weaponIcon.sprite = weaponItem.inventorySprite;

                // Set ItemIcon image component's alpha to 1
                Color c = weaponIcon.color;
                c.a = 1f;
                weaponIcon.color = c;

                // Get NameText TextMeshProUGUI component
                TextMeshProUGUI nameText = weaponSlot.transform.Find("NameText").GetComponent<TextMeshProUGUI>();

                // Set NameText 
                nameText.text = inventory.Container[i].item.itemName;
            }
            else
            {
                // Instantiate item grid slot
                GameObject itemSlot = Instantiate(itemGridSlotPrefab, itemGridContainer);

                // Add to local item slots list
                itemSlots.Add(itemSlot);

                // Populate slot with scriptable object data
                ItemSlotInteraction itemSlotInteraction = itemSlot.GetComponent<ItemSlotInteraction>();
                itemSlotInteraction.PopulateItemSlot(inventory.Container[i].item);
            }
        }
    }
}
