using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class NoteBox : MonoBehaviour
{
    // Singleton
    public static NoteBox instance;

    // Private References
    private CanvasGroup noteBoxCG;

    // Public References
    public TextMeshProUGUI noteText;
    public GameObject continueButton;

    // Public Variables
    [HideInInspector]
    public bool isActive;                                                                                       // Bool - Note box active

    public float noteDisplayDelay = 0.3f;                                                                       // Float - Delay time before displaying the note box
    public float noteResponseDisplayDelay = 0.3f;                                                               // Float - Delay time before displaying the response dialogue after note

    // Private Variables
    private string notePageText;                                                                                // String - Note page text
    
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
    }

    private void Initialize()
    {
        // Hide note box on start
        noteBoxCG.alpha = 0f;
        noteBoxCG.interactable = false;
        noteBoxCG.blocksRaycasts = false;
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
        Cursor.lockState = CursorLockMode.Locked;                                                   // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                     // Unlock mouse cursor

        isActive = true;

        // Fade in note box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(noteBoxCG, 0f, 1f, noteDisplayDelay));
        noteBoxCG.interactable = true;
        noteBoxCG.blocksRaycasts = true;

        noteText.text = notePageText;                                                               // Fill first page text
        continueButton.SetActive(true);                                                             // Show note box continue button
    }

    public void ShowNextNotePage()
    {                                                                                  
        Cursor.lockState = CursorLockMode.Locked;                                                   // Center and lock mouse cursor
        isActive = false;                                                                           // Set note box to inactive
        continueButton.SetActive(false);                                                            // Hide the note box continue button

        // Fade out note box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(noteBoxCG, noteBoxCG.alpha, 0f, 0f));

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
                        DialogueBox.instance.FillSentences(new string[] { obj.responseText });
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
                        DialogueBox.instance.FillSentences(new string[] { obj.responseText });
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                }
            }
        }

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
        
    }
}
