using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    // Private Cached References
    private UIReferences uiReferences;
    // Dialogue Box
    private CanvasGroup dialogueBoxCG;                                                              // Reference to the CanvasGroup of the dialogue box
    private TextMeshProUGUI dialogueText;                                                           // Reference to the text area of the dialogue box
    private GameObject dBoxContinueButton;                                                          // Reference to the continue button of the dialogue box

    // Note box
    private CanvasGroup noteBoxCG;                                                                  // Reference to the CanvasGroup of the note box
    private TextMeshProUGUI noteText;                                                               // Reference to the text area of the note box
    private GameObject noteContinueButton;                                                          // Reference to the continue button of the note box

    // Book display
    private CanvasGroup bookCG;                                                                     // Reference to the CanvasGroup of the book display
    private TextMeshProUGUI bookTextPage1;                                                          // Reference to the first page of the book text display
    private TextMeshProUGUI bookTextPage2;                                                          // Reference to the second page of the book text display
    private GameObject bookContinueButton;                                                          // Reference to the continue button of the book display

    // Public string arrays for dialogue, note, response and book texts
    [HideInInspector]
    public string[] dialogueTexts;                                                                  // String array to store dialogue box texts
    [HideInInspector]
    public string[] noteTexts;                                                                      // String array to store note box texts
    [HideInInspector]
    public string[] noteResponseTexts;                                                              // String array to store response texts after note box
    [HideInInspector]
    public string[] bookTexts;                                                                      // String array to store book texts

    // Public Variables
    public float typingSpeed;                                                                       // Text auto-type speed in milliseconds
    public float dialogueDisplayDelay = 0.1f;                                                       // Float to store the delay time in displaying the dialogue box
    public float noteDisplayDelay = 0.3f;                                                           // Float to store the delay time in displaying the note box after the initial dialogue
    public float noteResponseDisplayDelay = 0.3f;                                                   // Float to store the delay time in displaying the response dialogue after note
    public float bookDisplayDelay = 0.3f;                                                           // Float to store the delay time in displaying the book

    // Private Variables
    // Current indexes of the dialogue system boxes
    private int dialogueIndex = 0;                                                                  // Index to the current dialogue of the dialogue box
    private int noteIndex = 0;                                                                      // Index to the current page of the note box
    private int bookIndex = 0;                                                                      // Index to the current page (plus 1) of the book display

    // Bools updated for the current object
    private bool hasNote;                                                                           // Bool to store if object has a note after initial dialogue
    private bool hasResponseAfterNote;                                                              // Bool to store if object has a response dialogue after closing note box
    private bool hasBook;                                                                           // Bool to store if object has a book display

    // Dialogue System Configuration
    private bool isActive = false;                                                                  // Bool to store active status of dialogue system
    private bool isTyping = false;                                                                  // Bool to store typing status of dialogue box
    private bool skipAutoTyping = false;                                                            // Is set to true if player stops auto typing of current dialogue box text
    private bool skippedCurrentDialogue = false;                                                    // Bool to check whether player already skipped auto-typing dialogue
    private bool isShowingResponseAfterNote = false;                                                // Is true when response is being shown after note

    private void Start()
    {
        // Set references to UI elements
        uiReferences = GameSession.instance.uiReferences;
        dialogueBoxCG = uiReferences.dialogueBoxCanvasGroup;
        dialogueText = uiReferences.dialogueText;
        dBoxContinueButton = uiReferences.dBoxContinueButton;
        noteBoxCG = uiReferences.noteBoxCanvasGroup;
        noteText = uiReferences.noteText;
        noteContinueButton = uiReferences.noteContinueButton;
        bookCG = uiReferences.bookCanvasGroup;
        bookTextPage1 = uiReferences.bookTextPage1;
        bookTextPage2 = uiReferences.bookTextPage2;
        bookContinueButton = uiReferences.bookContinueButton;

        // Prevent the UI elements from being clickable
        dialogueBoxCG.interactable = false;
        dialogueBoxCG.blocksRaycasts = false;
        noteBoxCG.interactable = false;
        noteBoxCG.blocksRaycasts = false;
        bookCG.interactable = false;
        bookCG.blocksRaycasts = false;
    }

    private void Update()
    {
        if (isTyping)
        {
            if (dialogueText.text == dialogueTexts[dialogueIndex])
            {
                dBoxContinueButton.SetActive(true);                                                 // Show dialogue box continue button after auto-typing is finished
            }

            if (Input.GetKeyDown(KeyCode.Space) && !skippedCurrentDialogue)
            {
                dBoxContinueButton.SetActive(true);                                                 // Show dialogue box continue button if player skips the dialogue by pressing space
                skipAutoTyping = true;
                skippedCurrentDialogue = true;
            }
        }
        // TODO: Add ability to call OnClick of continue button on pressing space (preferably using the new InputSystem)
    }

    private void ResetValues()
    {
        dialogueIndex = 0;                                                                          
        noteIndex = 0;
        bookIndex = 0;
        hasNote = false;
        hasResponseAfterNote = false;
        hasBook = false;
        isTyping = false;
        skipAutoTyping = false;
        skippedCurrentDialogue = false;
        isActive = false;
        isShowingResponseAfterNote = false;
        dialogueBoxCG.interactable = false;                                                         // Prevent UI elements from being clickable
        dialogueBoxCG.blocksRaycasts = false;
        noteBoxCG.interactable = false;
        noteBoxCG.blocksRaycasts = false;
        bookCG.interactable = false;
        bookCG.blocksRaycasts = false;
    }

    private void SetActive(bool setActive)                                                          // Sets the active status of the dialogue system
    {
        isActive = setActive;
    }

    public bool IsActive()                                                                          // Returns the dialogue system's active status
    {
        return isActive;
    }

    public void FillSentences(string[] sentenceArray)                                               // Populate the dialogue texts string array
    {
        dialogueTexts = sentenceArray;
    }

    public void FillNoteTexts(string[] noteTextArray)                                               // Populate the note texts string array
    {
        noteTexts = noteTextArray;
    }
    
    public void FillNoteResponseTexts(string[] noteResponseTextArray)                               // Populate the response texts string array
    {
        noteResponseTexts = noteResponseTextArray;
    }

    public void FillBookTexts(string[] bookTextArray)                                               // Populate the book texts string array
    {
        bookTexts = bookTextArray;
    }

    public void StartDialogue(bool _hasNote, bool _hasResponse, bool _hasBook)                      // Start displaying the dialogueTexts array in the dialogue box
    {
        SetActive(true);                                                                            // Set dialogue system to active
        hasNote = _hasNote;                                                                         // Whether the current object has a note after dialogue
        hasResponseAfterNote = _hasResponse;                                                        // Whether the current object has a response after note
        hasBook = _hasBook;                                                                         // Whether the current object has a book display after dialogue
        Cursor.visible = true;                                                                      // Show cursor
        isTyping = true;                                                                            // Text is being typed in the dialogue box
        StartCoroutine(Type(dialogueDisplayDelay));                                                 // Start dialogue box typing coroutine
    }

    public void ShowNote()
    {
        SetActive(true);                                                                            // Set dialogue system to active

        // Fade in note box
        noteBoxCG.interactable = true;
        noteBoxCG.blocksRaycasts = true;
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(noteBoxCG, 0f, 1f, noteDisplayDelay));

        Cursor.visible = true;                                                                      // Show cursor
        noteText.text = noteTexts[0];                                                               // Fill note text        
        noteContinueButton.SetActive(true);                                                         // Show note box continue button
    }

    public void ShowResponseAfterNote()                                                             // Start displaying the response texts array after closing the note
    {
        isShowingResponseAfterNote = true;                                                          // Showing response texts in the dialogue box
        SetActive(true);                                                                            // Set dialogue system to active
        Cursor.visible = true;                                                                      // Show cursor
        isTyping = true;                                                                            // Text is being typed in the dialogue box
        StartCoroutine(Type(noteResponseDisplayDelay));                                             // Start dialogue box typing coroutine
    }

    public void ShowBook()
    {
        SetActive(true);

        // Fade in book display
        bookCG.interactable = true;
        bookCG.blocksRaycasts = true;
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(bookCG, 0f, 1f, bookDisplayDelay));

        Cursor.visible = true;
        bookTextPage1.text = bookTexts[bookIndex];
        if (bookTexts.Length >= 2)
        {
            bookTextPage2.text = bookTexts[bookIndex + 1];
        }
        bookContinueButton.SetActive(true);
    }

    private IEnumerator Type(float delay)
    {
        yield return new WaitForSeconds(delay);                                                     // Wait before displaying dialogue box

        // Fade in dialogue box
        dialogueBoxCG.interactable = true;
        dialogueBoxCG.blocksRaycasts = true;
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(dialogueBoxCG, 0f, 1f, dialogueDisplayDelay));   

        foreach (char letter in dialogueTexts[dialogueIndex].ToCharArray())
        {
            if (skipAutoTyping)
            {
                dialogueText.text = dialogueTexts[dialogueIndex];
                skipAutoTyping = false;
                break;
            }
            else
            {
                dialogueText.text += letter;                                                        // Auto-typing text              
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        isTyping = false;
    }

    public void NextDialogueSentence()
    {
        dBoxContinueButton.SetActive(false);                                                        // Hide dialogue box continue button

        if (dialogueIndex < dialogueTexts.Length - 1)                                               // More dialogue texts left to display
        {
            dialogueIndex++;
            dialogueText.text = "";
            StartCoroutine(Type(0f));                                                               // No delay in between dialogue sentences
        }
        else
        {                                                                                           // No more dialogue texts left to display
            dialogueText.text = "";

            SetActive(false);                                                                       // Set dialogue system to not active
            Cursor.visible = false;                                                                 // Hide cursor
            isTyping = false;                                                                       // Text is not being typed in the dialogue box
            dBoxContinueButton.SetActive(false);                                                    // Hide dialogue box continue button

            // Fade out the dialogue box
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(dialogueBoxCG, dialogueBoxCG.alpha, 0f, 0f));

            if (hasNote && !isShowingResponseAfterNote)                                             // Show note if bool is true and not currently showing response after note
            {
                skippedCurrentDialogue = false;
                ShowNote();
            }
            else if (hasBook)                                                                       // Show book is bool is true
            {
                ShowBook();
            }
            else
            {
                ResetValues();
            }
        }
    }

    public void NextNoteText()
    {
        if (noteIndex < noteTexts.Length - 1)                                                       // More note texts to display
        {
            noteIndex++;
            noteText.text = noteTexts[noteIndex];
        }
        else
        {                                                                                           // No more note texts to display
            Cursor.visible = false;                                                                 // Hide cursor
            SetActive(false);                                                                       // Set dialogue system to not active
            noteContinueButton.SetActive(false);                                                    // Hide the note box continue button

            // Fade out note box
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(noteBoxCG, noteBoxCG.alpha, 0f, 0f));                                                 

            if (hasResponseAfterNote)                                                               // Show response after note if bool is true
            {
                dialogueTexts = noteResponseTexts;                                                  // Populate the dialogueTexts array with the response texts
                dialogueIndex = 0;                                                                  // Set the dialogue box text index to 0
                ShowResponseAfterNote();
            }
            else
            {
                ResetValues();
            }
        }
    }

    public void NextBookTexts()
    {
        bookIndex += 2;                                                                             // Set index to 2 pages ahead since we display 2 pages at a time
        if (bookIndex < bookTexts.Length)
        {                                                                       
            bookTextPage1.text = bookTexts[bookIndex];
            if ((bookIndex + 1) < bookTexts.Length)
            {
                bookTextPage2.text = bookTexts[bookIndex + 1];
            }
            else
            {
                bookTextPage2.text = "";
            }
        }
        else
        {
            Cursor.visible = false;
            SetActive(false);
            bookContinueButton.SetActive(false);

            // Fade out book display
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(bookCG, bookCG.alpha, 0f, 0f));

            // TODO: add ability to view response after book if it exists
            ResetValues();
        }
    }
}
