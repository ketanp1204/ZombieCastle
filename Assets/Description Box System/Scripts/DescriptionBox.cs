using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionBox : MonoBehaviour
{
    // Singleton
    public static DescriptionBox instance;

    // Private References
    private CanvasGroup descBoxCG;

    // Public References
    public TextMeshProUGUI descText;
    public Image iconImage;

    // Public Variables
    [HideInInspector]
    public bool isActive;                                                                                           // Bool - If enabled, pressing escape will close description box

    // Private Variables
    private ItemObject currentItem = null;                                                                          // ItemObject - The current ItemObject which has the note box display

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
        descBoxCG = GetComponent<CanvasGroup>();
    }

    private void Initialize()
    {
        // Hide note box on start
        descBoxCG.alpha = 0f;
        descBoxCG.interactable = false;
        descBoxCG.blocksRaycasts = false;
    }

    private void SetCurrentItem(ItemObject item)
    {
        currentItem = item;

        SetIcon();

        SetDescriptionText();
    }

    private void SetIcon()
    {
        if (currentItem != null)
        {
            if (currentItem.itemType == ItemType.PC_Then_Inventory)
            {
                iconImage.sprite = ((PC_Then_Inventory_Object)currentItem).inventorySprite;
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
        }
    }

    public void ShowDescriptionBox(ItemObject item)
    {
        if (!instance.isActive)
        {
            SetCurrentItem(item);

            isActive = true;

            // Prevent escape key from closing inventory box
            InventoryManager.instance.SetDescBoxOpenFlag();

            // Play paper pickup sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PaperPickup);

            // Fade in description box
            //descBoxCG.alpha = 1f;
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(descBoxCG, 0f, 1f, 0f));
            descBoxCG.interactable = true;
            descBoxCG.blocksRaycasts = true;
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
            InventoryManager.instance.UnsetDescBoxOpenFlag();

            // Reset local values
            ResetValues();
        }
    }

    private void ResetValues()
    {
        isActive = false;
        descBoxCG.interactable = false;
        descBoxCG.blocksRaycasts = false; 
        currentItem = null;
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
