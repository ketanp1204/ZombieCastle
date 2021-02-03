using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerObjectSelection : MonoBehaviour
{
    // Private Cached References
    private Canvas dynamicUICanvas;
    private GameObject dialogueManager;
    private Collider2D collidedObject;

    private GameObject objectNameGO;
    private bool nameBoxReplaced = false;
    private bool triggerStay = false;
    private string previousObjectName;
    private string[] sentenceArray;
    private string[] noteTextsArray;
    private string[] noteResponseTextsArray;
    private string[] bookTextsArray;

    void OnEnable()
    {
        dynamicUICanvas = GameSession.instance.dynamicUICanvas;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && triggerStay)
        {
            dialogueManager = GameSession.instance.dialogueManager;
            Dialogue dialogue = dialogueManager.GetComponent<Dialogue>();

            if (!dialogue.IsActive())                                                   
            {
                objectNameGO.SetActive(false);                                                          // Hide the object name display

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
            collidedObject = collision;
            triggerStay = true;

            // Get name of newly found object
            string objectName = collision.GetComponent<ObjectProperties>().objectName;

            // Show object name
            if (objectNameGO != null)
            {
                if (!previousObjectName.Equals(objectName))
                {
                    previousObjectName = objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text;
                    Destroy(objectNameGO);
                    nameBoxReplaced = true;
                }
            }
            else
            {
                previousObjectName = "";
            }

            // Instantiate ObjectName prefab
            dynamicUICanvas = GameSession.instance.dynamicUICanvas;
            objectNameGO = Instantiate(GameAssets.instance.objectNamePrefab, dynamicUICanvas.transform);

            // Set ObjectName text
            objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text = objectName;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            triggerStay = false;

            if (!nameBoxReplaced)
            {
                Destroy(objectNameGO);
            }
            else
            {
                nameBoxReplaced = false;
                if (previousObjectName != "")
                {
                    objectNameGO.GetComponentInChildren<TextMeshProUGUI>().text = previousObjectName;
                }
            }
        }
    }
}
