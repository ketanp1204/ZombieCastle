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

            Debug.Log("interact");
            if (isMazePuzzleCollider)
            {
                // dialogue stuff before starting maze

                // Start maze puzzle game (TODO: have to refactor this to where the dialogue ends or to a button on the maze puzzle UI)
                // MazePuzzle.instance.StartMazePuzzle();

                // Load maze puzzle UI
                MazePuzzle.LoadMazePuzzleUI();

                triggerStay = false;
            }
            else
            {
                // normal "Object" tag interaction

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            triggerStay = true;

            // Enable Object Glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = true;

            // Show object name text popup
            popupTextUI.text = collision.GetComponent<ObjectProperties>().objectName;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));
        }

        if (collision.CompareTag("R1_MazePuzzle"))
        {
            triggerStay = true;

            // Set flag bool
            isMazePuzzleCollider = true;

            // Enable Object Glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = true;

            // Show object name text popup
            popupTextUI.text = collision.GetComponent<ObjectProperties>().objectName;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));
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

        if (collision.CompareTag("R1_MazePuzzle"))
        {
            triggerStay = false;

            // Unset flag bool
            isMazePuzzleCollider = false;

            // Disable Glow
            collision.GetComponent<SpriteGlow.SpriteGlowEffect>().enabled = false;

            // Hide object name text popup
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
        }
    }
}
