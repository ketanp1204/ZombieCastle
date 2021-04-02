using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteBox : MonoBehaviour
{
    // Singleton
    public static NoteBox instance;

    // Private References
    private CanvasGroup noteBoxCG;
    private Image backgroundImage;

    // Public References
    public TextMeshProUGUI noteText;
    public GameObject continueButton;

    // Public Variables
    [HideInInspector]
    public bool isActive;                                                                                       // Bool - Note box active

    public float noteDisplayDelay = 0.3f;                                                                       // Float - Delay time before displaying the note box
    public float noteResponseDisplayDelay = 0.3f;                                                               // Float - Delay time before displaying the response dialogue after note

    // Private Variables
    private bool isOpen = false;                                                                                // Bool - Note box is open
    private string notePageText;                                                                                // String - Note page text
    private bool continueButtonEnabled = false;                                                                 // Bool - If enabled, player can press space bar to press the continue button

    private ItemObject currentItem = null;                                                                      // ItemObject - The current ItemObject which has the note box display

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
        noteBoxCG = GetComponent<CanvasGroup>();
        backgroundImage = transform.Find("NoteBackground").GetComponent<Image>();
    }

    private void Initialize()
    {
        // Hide note box on start
        noteBoxCG.alpha = 0f;
        noteBoxCG.interactable = false;
        noteBoxCG.blocksRaycasts = false;
    }

    public static bool IsOpen()
    {
        if (instance == null)
            return false;

        return instance.isOpen;
    }

    public void FillNotePageText()
    {
        if (currentItem != null)
        {
            if (currentItem.itemType == ItemType.PC_Then_Note)
            {
                notePageText = ((PC_Then_Note_Object)currentItem).noteText;
            }
            else if(currentItem.itemType == ItemType.NoteOnly)
            {
                notePageText = ((NoteOnlyObject)currentItem).noteText;
            }
        }
    }

    public void SetCurrentItem(ItemObject item)
    {
        currentItem = item;
    }

    public void ShowNoteBox()
    {
        Player.StopMovement();
        Player.DisableAttackInput();
        PlayerTopDown.StopMovement();

        Cursor.lockState = CursorLockMode.Locked;                                                   // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                     // Unlock mouse cursor

        // Play paper pickup sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PaperPickup);

        // Prevent inventory and toolbar open
        InventoryManager.DisableInventoryOpen();
        ToolbarManager.DisableToolbarOpen();

        isActive = true;

        if (currentItem != null)
        {
            if (currentItem.itemType == ItemType.PC_Then_Note)
            {
                // Set UI element positions based on size of note
                if (((PC_Then_Note_Object)currentItem).largeNote)
                {
                    backgroundImage.sprite = GameAssets.instance.noteBoxLarge;
                    backgroundImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(702.5f, 900f);
                    noteText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 700f);
                    continueButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(209f, -387f);
                }
                else
                {
                    backgroundImage.sprite = GameAssets.instance.noteBoxSmall;
                    backgroundImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(750f, 596.27f);
                    noteText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 450f);
                    continueButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(209f, -223f);
                }
            }
        }

        // Fade in note box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(noteBoxCG, 0f, 1f, noteDisplayDelay));
        noteBoxCG.interactable = true;
        noteBoxCG.blocksRaycasts = true;

        isOpen = true;

        noteText.text = notePageText;                                                               // Fill first page text
        continueButton.SetActive(true);                                                             // Show note box continue button
        new Task(SetContinueButtonEnabledFlagToTrueAfterDelay());
    }

    public void CloseNoteBox()
    {
        // Play continue button sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.ContinueButton);

        Cursor.lockState = CursorLockMode.Locked;                                                   // Center and lock mouse cursor
        isActive = false;                                                                           // Set note box to inactive
        continueButton.SetActive(false);                                                            // Hide the note box continue button

        // Fade out note box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(noteBoxCG, noteBoxCG.alpha, 0f, 0f));

        isOpen = false;

        // Allow inventory and toolbar to open
        InventoryManager.EnableInventoryOpen();
        ToolbarManager.EnableToolbarOpen();

        // Showing response after note if it exists
        if (currentItem != null)
        {
            if (currentItem.itemType == ItemType.PC_Then_Note)
            {
                PC_Then_Note_Object obj = (PC_Then_Note_Object)currentItem;

                if (obj.hasResponseAfterNote)
                {
                    if (DialogueBox.instance)
                    {
                        DialogueBox.instance.SetCurrentItem(obj);
                        DialogueBox.instance.FillSentences(obj.responseTexts);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                }
            }
            else if (currentItem.itemType == ItemType.NoteOnly)
            {
                NoteOnlyObject obj = (NoteOnlyObject)currentItem;

                if (obj.hasResponseAfterNote)
                {
                    if (DialogueBox.instance)
                    {
                        DialogueBox.instance.SetCurrentItem(obj);
                        DialogueBox.instance.FillSentences(obj.responseTexts);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                }
            }
        }

        Player.EnableMovement();
        Player.EnableAttackInputAfterDelay();
        PlayerTopDown.EnableMovement();

        ResetValues();
    }

    private void ResetValues()
    {
        isActive = false;
        noteBoxCG.interactable = false;
        noteBoxCG.blocksRaycasts = false;
        currentItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (continueButtonEnabled)
        {
            // Click continue button by pressing space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                continueButton.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    private IEnumerator SetContinueButtonEnabledFlagToTrueAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);

        continueButtonEnabled = true;
    }
}
