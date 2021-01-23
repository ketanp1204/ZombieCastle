using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIReferences : MonoBehaviour
{
    [Header("General")]
    public Camera mainCamera;
    public GameObject pauseMenuUI;
    public GameObject dialogueManager;

    [Header("Canvases")]
    public Canvas staticUICanvas;
    public Canvas dynamicUICanvas;
    
    [Header("Dialogue Box")]
    public CanvasGroup dialogueBoxCanvasGroup;
    public TextMeshProUGUI dialogueText;
    public GameObject dBoxContinueButton;
    [Header("Note Box")]
    public CanvasGroup noteBoxCanvasGroup;
    public TextMeshProUGUI noteText;
    public GameObject noteContinueButton;
    [Header("Book Display")]
    public CanvasGroup bookCanvasGroup;
    public TextMeshProUGUI bookTextPage1;
    public TextMeshProUGUI bookTextPage2;
    public GameObject bookContinueButton;
}
