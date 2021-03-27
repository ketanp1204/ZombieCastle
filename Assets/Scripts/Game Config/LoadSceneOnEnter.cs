using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadSceneOnEnter : MonoBehaviour
{
    // Private references
    private UIReferences uiReferences;
    private TextMeshProUGUI popupTextUI;
    private SpriteGlow.SpriteGlowEffect glowEffect;

    // Public variables
    public string sceneName;

    // Private variables
    private bool checkForInput = false;
    

    void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
        popupTextUI = uiReferences.popupTextUI;
        glowEffect = GetComponent<SpriteGlow.SpriteGlowEffect>();
        glowEffect.enabled = false;
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
        if(checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (PlayerStats.isFirstScene)
                {
                    PlayerStats.isFirstScene = false;
                }

                // Hide door glow
                glowEffect.enabled = false;

                AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.DoorOpen);

                new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f, 0.1f));
                LevelManager.LoadSceneByName(sceneName);
            }
        }
    }
}
