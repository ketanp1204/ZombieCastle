using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    // Private Cached References
    private UIReferences uiReferences;
    // Dialogue Box
    private Animator dialogueBoxAnimator;                                   // Reference to the animator component of the dialogue box
    private TextMeshProUGUI dialogueText;                                   // Reference to the text area of the dialogue box
    private GameObject dBoxContinueButton;                                  // Reference to the continue button of the dialogue box

    // Note box
    private Animator noteAnimator;                                          // Reference to the animator component of the note box
    private TextMeshProUGUI noteText;                                       // Reference to the text area of the note box
    private GameObject noteContinueButton;                                  // Reference to the continue button of the note box

    // Public string arrays for dialogue, note and response texts
    [HideInInspector]
    public string[] dialogueTexts;                                          // String array to store dialogue box texts
    [HideInInspector]
    public string[] noteTexts;                                              // String array to store note box texts
    [HideInInspector]
    public string[] noteResponseTexts;                                      // String array to store response texts after note box

    // Public Variables
    public float typingSpeed;                                               // Text auto-type speed in milliseconds
    public float dialogueDisplayDelay = 0.5f;                               // Float to store the delay time in displaying the dialogue box
    public float noteDisplayDelay = 0.5f;                                   // Float to store the delay time in displaying the note box after the initial dialogue
    public float noteResponseDisplayDelay = 0.5f;                           // Float to store the delay time in displaying the response dialogue after note

    // Private Variables
    private int dialogueIndex = 0;                                          // Index to the current dialogue of the dialogue box
    private int noteIndex = 0;                                              // Index to the current page of the note box
    private bool hasNote;                                                   // Bool to store if object has a note after initial dialogue
    private bool hasResponseAfterNote;                                      // Bool to store if object has a response dialogue after closing note box
    private bool isTyping = false;                                          // Bool to store typing status of dialogue box
    private bool skipAutoTyping = false;                                    // Is set to true if player stops auto typing of current dialogue box text
    private bool skippedCurrentDialogue = false;                            // Bool to check whether player already skipped auto-typing dialogue
    private bool isActive = false;                                          // Bool to store active status of dialogue system
    private bool isShowingResponseAfterNote = false;                        // Is true when response is being shown after note

    private void Start()
    {
        // Set references to UI elements
        uiReferences = GameSession.instance.uiReferences;
        dialogueBoxAnimator = uiReferences.dialogueBoxAnimator;
        dialogueText = uiReferences.dialogueText;
        dBoxContinueButton = uiReferences.dBoxContinueButton;
        noteAnimator = uiReferences.noteAnimator;
        noteText = uiReferences.noteText;
        noteContinueButton = uiReferences.noteContinueButton;
    }

    private void Update()
    {
        if (isTyping)
        {
            if (dialogueText.text == dialogueTexts[dialogueIndex])
            {
                dBoxContinueButton.SetActive(true);                         // Show dialogue box continue button after auto-typing is finished
            }

            if (Input.GetKeyDown(KeyCode.Space) && !skippedCurrentDialogue)
            {
                dBoxContinueButton.SetActive(true);                         // Show dialogue box continue button if player skips the dialogue by pressing space
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
        hasNote = false;
        hasResponseAfterNote = false;
        isTyping = false;
        skipAutoTyping = false;
        skippedCurrentDialogue = false;
        isActive = false;
        isShowingResponseAfterNote = false;
    }

    private void SetActive(bool setActive)                                  // Sets the active status of the dialogue system
    {
        isActive = setActive;
    }

    public bool IsActive()                                                  // Returns the dialogue system's active status
    {
        return isActive;
    }

    public void FillSentences(string[] sentenceArray)                       // Populate the dialogue texts string array
    {
        dialogueTexts = sentenceArray;
    }

    public void FillNoteTexts(string[] noteTextArray)                       // Populate the note texts string array
    {
        noteTexts = noteTextArray;
    }

    
    public void FillNoteResponseTexts(string[] noteResponseTextArray)       // Populate the response texts string array
    {
        noteResponseTexts = noteResponseTextArray;
    }

    public void StartDialogue(bool popup, bool hasResponse)                 // Start displaying the dialogueTexts array in the dialogue box
    {
        SetActive(true);                                                    // Set dialogue system to active
        hasNote = popup;                                                    // Whether the current object has a note after dialogue
        hasResponseAfterNote = hasResponse;                                 // Whether the current object has a response after note
        Cursor.visible = true;                                              // Show cursor
        isTyping = true;                                                    // Text is being typed in the dialogue box
        StartCoroutine(Type(dialogueDisplayDelay));                         // Start dialogue box typing coroutine
    }

    public void ShowNote()
    {
        SetActive(true);                                                    // Set dialogue system to active
        StartCoroutine(DisplayNoteAfterDelay(noteDisplayDelay));            // Show note after delay
    }

    private IEnumerator DisplayNoteAfterDelay(float delay)                  // Wait before displaying note box
    {
        yield return new WaitForSeconds(delay);                             

        noteAnimator.Play("FadeIn");                                        // Show popup box
        Cursor.visible = true;                                              // Show cursor
        noteText.text = noteTexts[0];                                       // Fill note text        
        noteContinueButton.SetActive(true);                                 // Show note box continue button
    }

    public void ShowResponseAfterNote()                                     // Start displaying the response texts array after closing the note
    {
        isShowingResponseAfterNote = true;                                  // Showing response texts in the dialogue box
        SetActive(true);                                                    // Set dialogue system to active
        Cursor.visible = true;                                              // Show cursor
        isTyping = true;                                                    // Text is being typed in the dialogue box
        StartCoroutine(Type(noteResponseDisplayDelay));                     // Start dialogue box typing coroutine
    }

    private IEnumerator Type(float delay)
    {
        yield return new WaitForSeconds(delay);                             // Wait before displaying dialogue box
        dialogueBoxAnimator.Play("FadeIn");                                 // Show dialogue box

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
                dialogueText.text += letter;                                // Auto-typing text              
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        isTyping = false;
    }

    public void NextDialogueSentence()
    {
        dBoxContinueButton.SetActive(false);                                // Hide dialogue box continue button

        if (dialogueIndex < dialogueTexts.Length - 1)                       // More dialogue texts left to display
        {
            dialogueIndex++;
            dialogueText.text = "";
            StartCoroutine(Type(0f));                                       // No delay in between dialogue sentences
        }
        else
        {                                                                   // No more dialogue texts left to display
            dialogueText.text = "";

            SetActive(false);                                               // Set dialogue system to not active
            Cursor.visible = false;                                         // Hide cursor
            isTyping = false;                                               // Text is not being typed in the dialogue box
            dBoxContinueButton.SetActive(false);                            // Hide dialogue box continue button
            dialogueBoxAnimator.Play("FadeOut");                            // Fade out the dialogue box

            if (hasNote && !isShowingResponseAfterNote)                     // Show note if bool is true and not currently showing response after note
            {
                skippedCurrentDialogue = false;
                ShowNote();
            }
            else
            {
                ResetValues();
            }
        }
    }

    public void NextNoteText()
    {
        if (noteIndex < noteTexts.Length - 1)                               // More note texts to display
        {
            noteIndex++;
            noteText.text = noteTexts[noteIndex];
        }
        else
        {                                                                   // No more note texts to display
            Cursor.visible = false;                                         // Hide cursor
            SetActive(false);                                               // Set dialogue system to not active
            noteContinueButton.SetActive(false);                            // Hide the note box continue button
            noteAnimator.Play("FadeOut");                                   // Fade out note box

            if (hasResponseAfterNote)                                       // Show response after note if bool is true
            {
                dialogueTexts = noteResponseTexts;                          // Populate the dialogueTexts array with the response texts
                dialogueIndex = 0;                                          // Set the dialogue box text index to 0
                ShowResponseAfterNote();
            }
            else
            {
                ResetValues();
            }
        }
    }
}
