using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room2AccessBehaviour : MonoBehaviour
{
    // Private references
    private UIReferences uiReferences;
    private TextMeshProUGUI popupTextUI;
    private SpriteGlow.SpriteGlowEffect glowEffect;
    private BoxCollider2D doorCollider;

    // Public variables
    public string sceneName = "Room2";


    // Private variables
    private bool checkForInput = false;


    // Start is called before the first frame update
    void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
        popupTextUI = uiReferences.popupTextUI;
        glowEffect = GetComponent<SpriteGlow.SpriteGlowEffect>();
        glowEffect.enabled = false;
        doorCollider = GetComponent<BoxCollider2D>();

        // Check if room1 maze completed
        if (GameData.r1_mazePuzzleCompleted)
        {
            doorCollider.enabled = true;
        }
        else
        {
            doorCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSelectionArea"))
        {
            // Show door glow
            glowEffect.enabled = true;

            checkForInput = true;
            popupTextUI.text = "E - Open Door";
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f, 0.1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSelectionArea"))
        {
            // Hide door glow
            glowEffect.enabled = false;

            checkForInput = false;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
        }
    }

    void Update()
    {
        if (checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                LoadRoom2();
            }
        }
    }

    private void LoadRoom2()
    {
        // Hide door glow
        glowEffect.enabled = false;

        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.DoorOpen);

        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
        LevelManager.LoadSceneByName(sceneName);
    }
}
