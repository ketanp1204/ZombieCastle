using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadSceneOnEnter : MonoBehaviour
{
    public string sceneName;
    private bool checkForInput = false;
    private UIReferences uiReferences;
    private TextMeshProUGUI doorOpenInstructionText;

    void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
        doorOpenInstructionText = uiReferences.doorOpenInstruction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkForInput = true;
        new Task(UIAnimation.FadeTMProTextAfterDelay(doorOpenInstructionText, 0f, 1f, 0f, 0.1f));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        checkForInput = false;
        new Task(UIAnimation.FadeTMProTextAfterDelay(doorOpenInstructionText, 1f, 0f, 0f, 0.1f));
    }

    void Update()
    {
        if(checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                new Task(UIAnimation.FadeTMProTextAfterDelay(doorOpenInstructionText, 1f, 0f, 0f, 0.1f));
                LevelManager.LoadSceneByName(sceneName);
            }
        }
    }
}
