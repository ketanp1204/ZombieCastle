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

    [Header("Player Health Bar")]
    public CanvasGroup playerHealthBarCanvasGroup;
    public HealthBar playerHealthBar;

    [Header("Maze Puzzle")]
    public CanvasGroup mazeCloseButtonCanvasGroup;
    public CanvasGroup mazeUICanvasGroup;
    public Image mazeSwitch1;
    public Image mazeSwitch2;
    public TextMeshProUGUI mazeCountdownTimerText;
    public Button mazePuzzleInteractButton;
    public TextMeshProUGUI mazePuzzleInteractText;

    [Header("Spot the Differences Puzzle")]
    public CanvasGroup differencePuzzleCanvasGroup;
    public TextMeshProUGUI diffPuzzleCountdownTimerText;
    public Button diffPuzzleInteractButton;
    public TextMeshProUGUI diffPuzzleInteractText;

    [Header("Text Popup")]
    public TextMeshProUGUI popupTextUI;
}
