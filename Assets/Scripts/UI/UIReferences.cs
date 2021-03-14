using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIReferences : MonoBehaviour
{
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

    [Header("Inventory System")]
    public InventoryObject playerInventory;
    public InventoryManager inventoryManager;

    [Header("Player Health Bar")]
    public CanvasGroup playerHealthBarCanvasGroup;
    public HealthBar playerHealthBar;

    [Header("Maze Puzzle")]
    public CanvasGroup mazeCloseButtonCanvasGroup;
    public CanvasGroup mazeUICanvasGroup;
    public Image mazeSwitch1;
    public Image mazeSwitch2;
    public TextMeshProUGUI mazeCountdownTimerText;
    public Button interactButton;
    public TextMeshProUGUI interactText;

    [Header("Text Popup")]
    public TextMeshProUGUI popupTextUI;
}
