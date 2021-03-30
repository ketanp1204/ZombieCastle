using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    // Instance
    public static InventoryManager instance;

    // Public Cached References
    public GameObject weaponGridSlotPrefab;
    public GameObject itemGridSlotPrefab;
    public TextMeshProUGUI highlightText;

    // Private Cached References
    private List<GameObject> weaponSlots = new List<GameObject>();
    private List<WeaponSlotInteraction> weaponSlotInteractionScriptInstances = new List<WeaponSlotInteraction>();
    private List<GameObject> itemSlots = new List<GameObject>();
    private List<ItemSlotInteraction> itemSlotInteractionScriptInstances = new List<ItemSlotInteraction>();
    private CanvasGroup inventoryCanvasGroup;

    private Transform weaponGridContainer;
    private Transform itemGridContainer;

    // Private variables
    private bool canOpenInventory;
    private bool isInventoryOpen;
    private bool isDescBoxOpen;

    // Public variables
    [HideInInspector]
    public bool isDraggingItem = false;
    [HideInInspector]
    public bool isDraggingWeapon = false;

    [HideInInspector]
    public ItemSlotInteraction draggingItemSlotScript;
    [HideInInspector]
    public WeaponSlotInteraction draggingWeaponSlotScript;

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
        Initialize();

        // Fill inventory slots
        UpdateInventorySlots();
    }

    private void SetReferences()
    {
        weaponGridContainer = transform.GetChild(0).Find("WeaponGrid");
        itemGridContainer = transform.GetChild(0).Find("ItemsGrid");
        inventoryCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Initialize()
    {
        if (GameData.lobby_introDialogueSeen)
            canOpenInventory = true;
        else
            canOpenInventory = false;

        isInventoryOpen = false;
        isDescBoxOpen = false;

        // Hide inventory on start
        inventoryCanvasGroup.alpha = 0f;
        inventoryCanvasGroup.interactable = false;
        inventoryCanvasGroup.blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isInventoryOpen && !isDescBoxOpen)
                ShowInventory();
            else if (isInventoryOpen && !isDescBoxOpen)
                HideInventory();
        }

        /* NOTE:    In the Unity editor, the close inventory box function does not work as intended. 
         *          On clicking on the close button in the inventory box, the mouse cursor disappears as expected.
         *          But on pressing escape from inventory box to close the inventory, the mouse cursor does not disappear.
         *          However, in the game build, this function works as expected when pressing escape to close the box.
        */

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInventoryOpen && !isDescBoxOpen)
            {
                HideInventory();
            }
        }
    }

    public static void DisableInventoryOpen()
    {
        instance.canOpenInventory = false;
    }

    public static void EnableInventoryOpen()
    {
        instance.canOpenInventory = true;
    }

    public void SetDescBoxOpenFlag()
    {
        isDescBoxOpen = true;
    }

    public void UnsetDescBoxOpenFlag()
    {
        new Task(UnsetDescBoxOpenFlagAfterDelay());
    }

    private IEnumerator UnsetDescBoxOpenFlagAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        isDescBoxOpen = false;
    }

    public void ShowTextOnHighlightText(string text, float displayDelay, float hideDelay)
    {
        highlightText.text = text;
        new Task(ShowText(displayDelay, hideDelay));
    }

    private IEnumerator ShowText(float displayDelay, float hideDelay)
    {
        yield return new WaitForSeconds(displayDelay);

        new Task(UIAnimation.FadeTMProTextAfterDelay(highlightText, 0f, 1f, 0f));

        yield return new WaitForSeconds(hideDelay);

        new Task(UIAnimation.FadeTMProTextAfterDelay(highlightText, 1f, 0f, 0f));
    }

    public static void ShowInventory()
    {
        if (instance.canOpenInventory)
        {
            if (!instance.isInventoryOpen)
            {
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

                // Stop player movement and attacks
                if (Player.instance)
                {
                    Player.StopMovement();
                    Player.DisableAttackInput();
                }
                else if (PlayerTopDown.instance)
                {
                    PlayerTopDown.StopMovement();
                }

                // Disable opening pause menu on pressing escape key
                PauseMenu.instance.CanPauseGame = false;
            }
        }
    }

    public static void HideInventory()
    {
        if (instance.isInventoryOpen)
        {
            instance.isInventoryOpen = false;

            // Play inventory close sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.InventoryClose);

            // Hide inventory display
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(instance.inventoryCanvasGroup, 1f, 0f, 0f, 0.3f));
            // Prevent mouse interaction
            instance.inventoryCanvasGroup.interactable = false;
            instance.inventoryCanvasGroup.blocksRaycasts = false;

            // Hide highlight text 
            instance.highlightText.text = "";
            instance.highlightText.alpha = 0f;

            // Lock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;

            // Resume player attack input after delay
            Player.EnableAttackInputAfterDelay();

            // Resume player movement after delay
            new Task(instance.ResumePlayerMovementAfterDelay(0.6f));

            // Enable opening pause menu on pressing escape key
            instance.StartCoroutine(PauseMenu.EnableCanPauseGameBoolAfterDelay(0.1f));
        }
    }

    private IEnumerator ResumePlayerMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Resume player movement
        if (Player.instance)
        {
            Player.EnableMovement();
        }
        else if (PlayerTopDown.instance)
        {
            PlayerTopDown.EnableMovement();
        }
    }

    public void AddInventoryItem(ItemObject item)
    {
        // Add to inventory scriptable object
        GameData.currentPlayerInventory.AddItem(item, 1);

        // Update inventory slots
        UpdateInventorySlots();

        if (SceneManager.GetActiveScene().name == "Room3")
        {
            DisableAllWeaponInteractions();
        }
    }

    public void DeleteInventoryItem(ItemObject item)
    {
        // Remove from inventory scriptable object
        GameData.currentPlayerInventory.RemoveItem(item);

        // Update inventory slots
        UpdateInventorySlots();
    }

    public bool ContainsItem(ItemObject item)
    {
        bool itemAlreadyExists = false;

        if (item.itemType == ItemType.Weapon)
        {
            foreach (Transform child in weaponGridContainer.transform)
            {
                WeaponSlotInteraction instance = child.GetComponent<WeaponSlotInteraction>();

                if (instance.scriptableObject == item)
                {
                    itemAlreadyExists = true;
                }
            }
        }
        else
        {
            foreach (Transform child in itemGridContainer.transform)
            {
                ItemSlotInteraction instance = child.GetComponent<ItemSlotInteraction>();

                if (instance.itemScriptableObject == item)
                {
                    itemAlreadyExists = true;
                }
            }
        }

        return itemAlreadyExists;
    }

    public void HighlightWeaponBeforeCombatStart(ItemObject item)
    {
        DisableInteractionOfAllSlots();

        bool itemFound = false;

        foreach (Transform child in weaponGridContainer.transform)
        {
            WeaponSlotInteraction instance = child.GetComponent<WeaponSlotInteraction>();

            if (instance.scriptableObject == item)
            {
                itemFound = true;

                instance.Highlight();
                instance.SetWeaponHighlightBeforeCombatStartFlag();
                instance.EnableInteraction();
                string selectWeaponText = "Select " + item.itemName;
                highlightText.text = selectWeaponText;
                highlightText.alpha = 1f;
                break;
            }
        }

        if (!itemFound)
        {
            Debug.Log("Weapon not found");
            EnableInteractionOfAllSlots();
        }
    }

    public void HighlightItemOnTreasureBoxInteraction(ItemObject item, TreasureBoxInteraction scriptInstance)
    {
        bool itemFound = false;

        // Disable weapon slot interaction
        foreach (Transform child in weaponGridContainer.transform)
        {
            WeaponSlotInteraction instance = child.GetComponent<WeaponSlotInteraction>();

            instance.HideSelectedBackground();
            instance.DisableInteraction();
        }

        // Look for item inside item slots
        foreach (Transform child in itemGridContainer.transform)
        {
            ItemSlotInteraction instance = child.GetComponent<ItemSlotInteraction>();

            if (instance.itemScriptableObject == item)
            {
                itemFound = true;

                instance.Highlight();
                instance.SetTreasureBoxInteractionFlag();
                instance.SetTreasureBoxInteractionScriptInstance(scriptInstance);
                instance.EnableInteraction();

                // In the future, remove this hard coded dependency
                PC_Then_Inventory_Object keyObject = (PC_Then_Inventory_Object)item;

                string selectKeyText = "Select " + keyObject.inventoryItemName;
                highlightText.text = selectKeyText;
                highlightText.alpha = 1f;
            }
            else
            {
                instance.HideSelectedBackground();
                instance.DisableInteraction();
            }
        }

        // Re-enable interaction of other slots if item not found in inventory
        if (!itemFound)
        {
            Debug.Log("Item not found in inventory");
            EnableInteractionOfAllSlots();
        }
    }

    public void EnableInteractionOfAllSlots()
    {
        foreach (Transform child in weaponGridContainer.transform)
        {
            WeaponSlotInteraction instance = child.GetComponent<WeaponSlotInteraction>();
            instance.EnableInteraction();
        }
        foreach (Transform child in itemGridContainer.transform)
        {
            ItemSlotInteraction instance = child.GetComponent<ItemSlotInteraction>();
            instance.EnableInteraction();
        }
    }

    public void DisableInteractionOfAllSlots()
    {
        foreach (Transform child in weaponGridContainer.transform)
        {
            WeaponSlotInteraction instance = child.GetComponent<WeaponSlotInteraction>();
            instance.DisableInteraction();
        }
        foreach (Transform child in itemGridContainer.transform)
        {
            ItemSlotInteraction instance = child.GetComponent<ItemSlotInteraction>();
            instance.DisableInteraction();
        }
    }

    public void DisableAllWeaponInteractions()
    {
        foreach (Transform child in weaponGridContainer.transform)
        {
            WeaponSlotInteraction instance = child.GetComponent<WeaponSlotInteraction>();
            instance.DisableInteraction();
        }
    }

    public void UpdateInventorySlots()
    {
        // Check and remove already deleted slots
        if (weaponSlots.Count != 0)
        {
            Dictionary<int, bool> slotExistsList = new Dictionary<int, bool>();

            for (int i = 0; i < weaponSlots.Count; i++)
            {
                slotExistsList[i] = false;
            }

            for (int i = 0; i < weaponSlotInteractionScriptInstances.Count; i++)
            {
                for (int j = 0; j < GameData.currentPlayerInventory.Container.Count; j++)
                {
                    ItemObject inventoryItem = GameData.currentPlayerInventory.Container[j].item;

                    if (weaponSlotInteractionScriptInstances[i].scriptableObject == inventoryItem)
                    {
                        slotExistsList[i] = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < slotExistsList.Count; i++)
            {
                if (slotExistsList[i] == false)
                {
                    Destroy(weaponSlots[i]);
                    weaponSlots.RemoveAt(i);
                    weaponSlotInteractionScriptInstances.RemoveAt(i);
                }
            }
        }

        if (itemSlots.Count != 0)
        {
            Dictionary<int, bool> slotExistsList = new Dictionary<int, bool>();

            for (int i = 0; i < itemSlots.Count; i++)
            {
                slotExistsList[i] = false;
            }

            for (int i = 0; i < itemSlotInteractionScriptInstances.Count; i++)
            {
                for (int j = 0; j < GameData.currentPlayerInventory.Container.Count; j++)
                {
                    ItemObject inventoryItem = GameData.currentPlayerInventory.Container[j].item;

                    if (itemSlotInteractionScriptInstances[i].itemScriptableObject == inventoryItem)
                    {
                        slotExistsList[i] = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < slotExistsList.Count; i++)
            {
                if (slotExistsList[i] == false)
                {
                    Destroy(itemSlots[i]);
                    itemSlots.RemoveAt(i);
                    itemSlotInteractionScriptInstances.RemoveAt(i);
                }
            }
        }
        

        // Update existing slots
        for (int i = 0; i < GameData.currentPlayerInventory.Container.Count; i++)
        {
            // Check type of item : Add to weapon grid if weapon otherwise into item grid
            if (GameData.currentPlayerInventory.Container[i].item.itemType == ItemType.Weapon)
            {
                // Cast item to WeaponObject
                WeaponObject weaponItem = (WeaponObject)GameData.currentPlayerInventory.Container[i].item;

                // Add weapon if it does not already exist in inventory
                if (ContainsItem(weaponItem) == false)
                {
                    // Instantiate weapon grid slot
                    GameObject weaponSlot = Instantiate(weaponGridSlotPrefab, weaponGridContainer);

                    // Add to local weapon slots list
                    weaponSlots.Add(weaponSlot);

                    // Get WeaponSlotInteraction script component
                    WeaponSlotInteraction interactionScript = weaponSlot.GetComponent<WeaponSlotInteraction>();

                    // Add to local weapon slot interaction script instances list
                    weaponSlotInteractionScriptInstances.Add(interactionScript);

                    // Set the weapon type and scriptable object on the interaction script
                    interactionScript.weaponType = weaponItem.weaponType;
                    interactionScript.scriptableObject = weaponItem;

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
                    nameText.text = GameData.currentPlayerInventory.Container[i].item.itemName;
                }
            }
            else
            {
                // Load the ItemObject                
                ItemObject item = GameData.currentPlayerInventory.Container[i].item;

                // Add item if it does not already exist in inventory
                if (ContainsItem(item) == false)
                {
                    // Instantiate item grid slot
                    GameObject itemSlot = Instantiate(itemGridSlotPrefab, itemGridContainer);

                    // Add to local item slots list
                    itemSlots.Add(itemSlot);

                    // Populate slot with scriptable object data
                    ItemSlotInteraction itemSlotInteraction = itemSlot.GetComponent<ItemSlotInteraction>();

                    // Add to local item slot interaction script instances list
                    itemSlotInteractionScriptInstances.Add(itemSlotInteraction);

                    itemSlotInteraction.PopulateItemSlot(GameData.currentPlayerInventory.Container[i].item);
                }
            }
        }
    }
}
