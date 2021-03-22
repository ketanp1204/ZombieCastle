using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSelection : MonoBehaviour
{
    // Private Cached References
    private UIReferences uiReferences;

    private TextMeshProUGUI popupTextUI;

    private Collider2D collidedObject;

    // Private variables
    private bool triggerStay = false;

    private bool isMazePuzzleCollider = false;
    private bool isJigsawPuzzleCollider = false;
    private bool isDiffPuzzleCollider = false;

    // Start is called before the first frame update
    void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
        popupTextUI = uiReferences.popupTextUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && triggerStay)
        {
            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

            triggerStay = false;

            if (isMazePuzzleCollider)
            {
                ObjectProperties objectProperties = collidedObject.GetComponent<ObjectProperties>();

                // Show dialogue before starting maze puzzle
                if (objectProperties.objectData != null)
                {
                    PlayerCommentOnlyObject obj = (PlayerCommentOnlyObject)objectProperties.objectData;

                    string[] sentenceArray = obj.playerComments;

                    if (DialogueBox.instance)
                    {
                        DialogueBox.instance.SetCurrentItem(obj);
                        DialogueBox.instance.SetMazePuzzleFlag();
                        DialogueBox.instance.FillSentences(sentenceArray);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                    else
                    {
                        Debug.Log("Dialogue box not found");
                    }
                }
                else
                {
                    Debug.Log("No scriptable object set in object properties");
                }
            }
            else if (isJigsawPuzzleCollider)
            {
                ObjectProperties objectProperties = collidedObject.GetComponent<ObjectProperties>();

                // Show dialogue before starting maze puzzle
                if (objectProperties.objectData != null)
                {
                    PlayerCommentOnlyObject obj = (PlayerCommentOnlyObject)objectProperties.objectData;

                    string[] sentenceArray = obj.playerComments;

                    if (DialogueBox.instance)
                    {
                        DialogueBox.instance.SetCurrentItem(obj);
                        DialogueBox.instance.SetJigsawPuzzleFlag();
                        DialogueBox.instance.FillSentences(sentenceArray);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                    else
                    {
                        Debug.Log("Dialogue box not found");
                    }
                }
                else
                {
                    Debug.Log("No scriptable object set in object properties");
                }
            }
            else if (isDiffPuzzleCollider)
            {
                
                ObjectProperties objectProperties = collidedObject.GetComponent<ObjectProperties>();

                // Show dialogue before starting diff puzzle
                if (objectProperties.objectData != null)
                {
                    PlayerCommentOnlyObject obj = (PlayerCommentOnlyObject)objectProperties.objectData;

                    string[] sentenceArray = obj.playerComments;

                    if (DialogueBox.instance)
                    {
                        DialogueBox.instance.SetCurrentItem(obj);
                        DialogueBox.instance.SetDiffPuzzleFlag();
                        DialogueBox.instance.FillSentences(sentenceArray);
                        DialogueBox.instance.StartDialogueDisplay();
                    }
                    else
                    {
                        Debug.Log("Dialogue box not found");
                    }
                }
            }
            else
            {
                // Normal "Object" tag interaction
                ObjectProperties objectProperties = collidedObject.GetComponent<ObjectProperties>();

                if (objectProperties.objectData != null)
                {
                    ItemObject itemScriptableObject = objectProperties.objectData;

                    if (itemScriptableObject.itemType == ItemType.PC_Only)
                    {
                        PlayerCommentOnlyObject pc_Only_Object = (PlayerCommentOnlyObject)itemScriptableObject;

                        string[] sentenceArray = pc_Only_Object.playerComments;

                        if (DialogueBox.instance)
                        {
                            DialogueBox.instance.SetCurrentItem(pc_Only_Object);
                            DialogueBox.instance.FillSentences(sentenceArray);
                            DialogueBox.instance.StartDialogueDisplay();
                        }
                        else
                        {
                            Debug.Log("Dialogue box not found");
                        }
                    }
                    else if (itemScriptableObject.itemType == ItemType.PC_Then_Inventory)
                    {
                        PC_Then_Inventory_Object pc_Then_Inventory_Object = (PC_Then_Inventory_Object)itemScriptableObject;

                        objectProperties.UpdateGameDataForPCThenInventoryObject(pc_Then_Inventory_Object);

                        string[] sentenceArray = pc_Then_Inventory_Object.playerComments;

                        if (DialogueBox.instance)
                        {
                            DialogueBox.instance.SetPCThenInventoryGameObject(collidedObject.gameObject);
                            DialogueBox.instance.SetCurrentItem(pc_Then_Inventory_Object);
                            DialogueBox.instance.FillSentences(sentenceArray);
                            DialogueBox.instance.SetInventoryAfterDialogueFlag();
                            DialogueBox.instance.StartDialogueDisplay();
                        }
                        else
                        {
                            Debug.Log("Dialogue box not found");
                        }
                    }
                    else if (itemScriptableObject.itemType == ItemType.PC_Then_Note)
                    {
                        PC_Then_Note_Object pc_Then_Note_Object = (PC_Then_Note_Object)itemScriptableObject;

                        string[] sentenceArray = pc_Then_Note_Object.playerComments;

                        if (DialogueBox.instance)
                        {
                            DialogueBox.instance.SetCurrentItem(pc_Then_Note_Object);
                            DialogueBox.instance.FillSentences(sentenceArray);
                            DialogueBox.instance.SetNoteAfterDialogueFlag();
                            DialogueBox.instance.StartDialogueDisplay();
                        }
                        else
                        {
                            Debug.Log("Dialogue box not found");
                        }
                    }
                }
                else
                {
                    Debug.Log("No scriptable object set in object properties");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            triggerStay = true;

            // Store collided object
            collidedObject = collision;

            // Enable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = true;

            // Show object name text popup
            popupTextUI.text = collision.GetComponent<ObjectProperties>().objectData.itemName;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));
        }
        else if (collision.CompareTag("R1_MazePuzzle"))
        {
            triggerStay = true;

            // Store collided object
            collidedObject = collision;

            // Enable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = true;

            // Show object name text popup
            popupTextUI.text = collision.GetComponent<ObjectProperties>().objectData.itemName;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));

            // Set flag bool
            isMazePuzzleCollider = true;
        }
        else if (collision.CompareTag("R2_JigsawPuzzle"))
        {
            triggerStay = true;

            // Store collided object
            collidedObject = collision;

            // Enable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = true;

            // Show object name text popup
            popupTextUI.text = collision.GetComponent<ObjectProperties>().objectData.itemName;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));

            // Set flag bool
            isJigsawPuzzleCollider = true;
        }
        else if (collision.CompareTag("R3_DiffPuzzle"))
        {
            triggerStay = true;

            // Store collided object
            collidedObject = collision;

            // Enable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = true;

            // Show object name text popup
            popupTextUI.text = collision.GetComponent<ObjectProperties>().objectData.itemName;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));

            // Set flag bool
            isDiffPuzzleCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Object"))
        {
            triggerStay = false;

            // Disable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = false;

            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
        }
        else if (collision.CompareTag("R1_MazePuzzle"))
        {
            triggerStay = false;

            // Disable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = false;

            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

            // Unset flag bool
            isMazePuzzleCollider = false;
        }
        else if (collision.CompareTag("R2_JigsawPuzzle"))
        {
            triggerStay = false;

            // Disable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = false;

            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

            // Unset flag bool
            isJigsawPuzzleCollider = false;
        }
        else if (collision.CompareTag("R3_DiffPuzzle"))
        {
            triggerStay = false;

            // Disable object glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = false;

            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));

            // Unset flag bool
            isDiffPuzzleCollider = false;
        }
    }
}
