using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionBox : MonoBehaviour
{
    public enum DescBoxOpenLocations
    {
        Null,
        ItemFromInventory,
        AfterReward
    }

    public enum UsableItems
    {
        Aspirin
    }

    // Singleton
    public static DescriptionBox instance;

    // Private References
    private UIReferences uIReferences;
    private CanvasGroup descBoxCG;
    private Button useButton;
    private HealthBar playerHealthBar;

    // Public References
    public TextMeshProUGUI newItemDiscoveredText;
    public TextMeshProUGUI objectNameText;
    public TextMeshProUGUI descText;
    public Image iconImage;
    public GameObject useButtonGO;

    // Public Variables
    [HideInInspector]
    public bool isActive;                                                                                           // Bool - If enabled, pressing escape will close description box

    // Private Variables
    private bool isOpen = false;                                                                                    // Bool - Desc box is open
    private ItemObject currentItem = null;                                                                          // ItemObject - The current ItemObject which has the note box display
    private DescBoxOpenLocations descBoxOpenLocation = DescBoxOpenLocations.Null;                                   // For different behaviour of desc box
    private string[] dialogueAfterReceivingReward = null;                                                           // String array - Dialogue (if any) after receiving reward on the desc box

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
    }

    private void SetReferences()
    {
        uIReferences = GameSession.instance.uiReferences;
        descBoxCG = GetComponent<CanvasGroup>();
        useButton = useButtonGO.GetComponent<Button>();
        playerHealthBar = uIReferences.playerHealthBar;
    }

    private void Initialize()
    {
        // Hide note box on start
        descBoxCG.alpha = 0f;
        descBoxCG.interactable = false;
        descBoxCG.blocksRaycasts = false;
    }

    public static bool IsOpen()
    {
        return instance.isOpen;
    }

    private void SetCurrentItem(ItemObject item)
    {
        currentItem = item;

        SetIcon();

        SetItemNameText();

        SetDescriptionText();

        SetUsableButton();
    }

    private void SetIcon()
    {
        if (currentItem != null)
        {
            if (currentItem.itemType == ItemType.PC_Then_Inventory)
            {
                iconImage.sprite = ((PC_Then_Inventory_Object)currentItem).inventorySprite;
            }
            else if (currentItem.itemType == ItemType.Weapon)
            {
                iconImage.sprite = ((WeaponObject)currentItem).inventorySprite;
            }
            else if (currentItem.itemType == ItemType.DescBox_Then_Dialogue)
            {
                iconImage.sprite = ((DescBox_Then_Dialogue_Object)currentItem).inventorySprite;
            }
        }
    }

    private void SetItemNameText()
    {
        if (currentItem != null)
        {
            if (currentItem.itemType == ItemType.PC_Then_Inventory)
            {
                objectNameText.text = ((PC_Then_Inventory_Object)currentItem).inventoryItemName;
            }
            else if (currentItem.itemType == ItemType.Weapon)
            {
                objectNameText.text = currentItem.itemName;
            }
            else if (currentItem.itemType == ItemType.DescBox_Then_Dialogue)
            {
                objectNameText.text = ((DescBox_Then_Dialogue_Object)currentItem).inventoryItemName;
            }
        }
    }

    private void SetDescriptionText()
    {
        if (currentItem != null)
        {
            if (currentItem.itemType == ItemType.PC_Then_Inventory)
            {
                descText.text = ((PC_Then_Inventory_Object)currentItem).itemDescription;
            }
            else if (currentItem.itemType == ItemType.Weapon)
            {
                descText.text = ((WeaponObject)currentItem).itemDescription;
            }
            else if (currentItem.itemType == ItemType.DescBox_Then_Dialogue)
            {
                descText.text = ((DescBox_Then_Dialogue_Object)currentItem).itemDescription;
            }
        }
    }

    private void SetUsableButton()
    {
        if (descBoxOpenLocation != DescBoxOpenLocations.AfterReward)
        {
            if (currentItem.itemType == ItemType.DescBox_Then_Dialogue)
            {
                DescBox_Then_Dialogue_Object obj = (DescBox_Then_Dialogue_Object)currentItem;

                if (obj.inventoryItemName == UsableItems.Aspirin.ToString())
                {
                    // Set usable button for health potion
                    useButtonGO.SetActive(true);
                    useButton.onClick.AddListener(() => playerHealthBar.UseHealthPotion());
                }
            }
        }
    }


    public void ShowDescBoxFromInventory(ItemObject item)
    {
        if (!instance.isActive)
        {
            descBoxOpenLocation = DescBoxOpenLocations.ItemFromInventory;

            SetCurrentItem(item);

            isActive = true;

            // Prevent escape key from closing inventory box
            InventoryManager.instance.SetDescBoxOpenFlag();
            
            // Play paper pickup sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PaperPickup);

            // Fade in description box
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(descBoxCG, 0f, 1f, 0f));
            descBoxCG.interactable = true;
            descBoxCG.blocksRaycasts = true;

            isOpen = true;
        }
    }

    // When receiving a new item from a treasure box or a puzzle
    public void ShowRewardInDescBoxAfterDelay(float delay, ItemObject item, string[] dialogueAfterReward, AudioManager.Sound itemReceivedSound)
    {
        new Task(ShowDescBoxRewardAfterDelay(delay, item, dialogueAfterReward, itemReceivedSound));
    }

    private IEnumerator ShowDescBoxRewardAfterDelay(float delay, ItemObject item, string[] dialogueAfterReward, AudioManager.Sound itemReceivedSound)
    {
        Player.StopMovement();
        Player.DisableAttackInput();

        yield return new WaitForSeconds(delay);

        if (!instance.isActive)
        {
            descBoxOpenLocation = DescBoxOpenLocations.AfterReward;

            SetCurrentItem(item);

            if (dialogueAfterReward != null)
            {
                dialogueAfterReceivingReward = dialogueAfterReward;
            }
            else
            {
                dialogueAfterReceivingReward = null;
            }

            isActive = true;

            // Prevent escape key from closing inventory box
            InventoryManager.instance.SetDescBoxOpenFlag();

            // Play item received sound
            if (itemReceivedSound != AudioManager.Sound.Null)
            {
                AudioManager.PlaySoundOnceOnNonPersistentObject(itemReceivedSound);
            }

            // Unlock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;

            // Add reward to inventory
            if (InventoryManager.instance)
            {
                InventoryManager.instance.AddInventoryItem(item);
            }

            // Highlight inventory bag in toolbar
            if (ToolbarManager.instance)
            {
                ToolbarManager.instance.HighlightInventoryBag();
            }

            // Fade in new item discovered text:
            new Task(UIAnimation.FadeTMProTextAfterDelay(newItemDiscoveredText, 0f, 1f, 0f));

            // Play paper pickup sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PaperPickup);

            // Fade in description box
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(descBoxCG, 0f, 1f, 0.5f));       // TODO: tweak delay value
            descBoxCG.interactable = true;
            descBoxCG.blocksRaycasts = true;

            isOpen = true;

            // Disable opening pause menu on pressing escape key
            PauseMenu.instance.CanPauseGame = false;
        }
    }

    public void CloseDescriptionBox()
    {
        if (instance.isActive)
        {
            // Play paper pickup sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PaperPickup);

            // Hide description box
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(descBoxCG, 1f, 0f, 0f));
            descBoxCG.interactable = false;
            descBoxCG.blocksRaycasts = false;

            // Enable escape key to close inventory box
            if (descBoxOpenLocation == DescBoxOpenLocations.AfterReward)
            {
                if (dialogueAfterReceivingReward != null)
                {
                    if (DialogueBox.instance)
                    {
                        // Show dialogue after reward
                        DialogueBox.instance.FillSentences(dialogueAfterReceivingReward);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                }

                // Lock mouse cursor
                Cursor.lockState = CursorLockMode.Locked;

                // Fade out new item discovered text:
                new Task(UIAnimation.FadeTMProTextAfterDelay(newItemDiscoveredText, 1f, 0f, 0f));

                InventoryManager.instance.UnsetDescBoxOpenFlag();

                // Enable opening pause menu on pressing escape key
                instance.StartCoroutine(PauseMenu.EnableCanPauseGameBoolAfterDelay(0.1f));

                Player.EnableMovement();
                Player.EnableAttackInputAfterDelay();
            }
            else if (descBoxOpenLocation == DescBoxOpenLocations.ItemFromInventory)
            {
                InventoryManager.instance.UnsetDescBoxOpenFlag();
            }

            // Reset local values
            ResetValues();
        }
    }

    private void ResetValues()
    {
        isActive = false;
        isOpen = false;
        descBoxCG.interactable = false;
        descBoxCG.blocksRaycasts = false; 
        currentItem = null;
        useButton.onClick.RemoveAllListeners();
        useButtonGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseDescriptionBox();
            }
        }
    }
}
