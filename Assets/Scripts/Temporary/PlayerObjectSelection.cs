﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerObjectSelection : MonoBehaviour
{
    // Private Cached References
    private UIReferences uiReferences;

    private TextMeshProUGUI popupTextUI;

    private GameObject dialogueManager;

    private Collider2D collidedObject;

    private Dialogue dialogue;

    private bool triggerStay = false;

    private string[] sentenceArray;
    private string[] noteTextsArray;
    private string[] noteResponseTextsArray;
    private string[] bookTextsArray;

    private void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && triggerStay)
        {
            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

            dialogueManager = GameSession.instance.dialogueManager;
            dialogue = dialogueManager.GetComponent<Dialogue>();

            if (!dialogue.IsActive())                                                   
            {
                ObjectProperties objectProperties = collidedObject.GetComponent<ObjectProperties>();
                sentenceArray = new string[] { objectProperties.objectData.playerComment };

                // Fill response
                bool hasNote = objectProperties.objectData.hasNote;
                bool hasResponseAfterNote = objectProperties.objectData.hasResponseAfterNote;
                bool hasBook = objectProperties.objectData.hasBookDisplay;

                if (hasNote)
                {
                    noteTextsArray = new string[] { objectProperties.objectData.noteText };
                    dialogue.FillNoteTexts(noteTextsArray);
                }
                if (hasResponseAfterNote)
                {
                    noteResponseTextsArray = new string[] { objectProperties.objectData.responseText };
                    dialogue.FillNoteResponseTexts(noteResponseTextsArray);
                }
                if (hasBook)
                {
                    bookTextsArray = objectProperties.objectData.bookTexts;
                    dialogue.FillBookTexts(bookTextsArray);
                }

                dialogue.FillSentences(sentenceArray);
                dialogue.StartDialogue(hasNote, hasResponseAfterNote, hasBook);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            triggerStay = true;

            // Store the collided object
            collidedObject = collision;

            // Show object name text popup
            popupTextUI = uiReferences.popupTextUI;
            popupTextUI.text = collision.GetComponent<ObjectProperties>().objectName;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));

            // Enable Glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            triggerStay = false;

            // Disable Glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = false;

            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
        }
    }
}
