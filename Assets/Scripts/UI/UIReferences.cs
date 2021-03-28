using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIReferences : MonoBehaviour
{
    [Header("Player Health Bar")]
    public HealthBar playerHealthBar;

    [Header("Maze Puzzle")]
    public CanvasGroup mazeUICanvasGroup;
    public Image mazeSwitch1;
    public Image mazeSwitch2;
    public TextMeshProUGUI mazeCountdownTimerText;
    public Button mazePuzzleInteractButton;
    public TextMeshProUGUI mazePuzzleInteractText;

    [Header("Jigsaw Puzzle")]
    public CanvasGroup jigsawPuzzleCanvasGroup;
    public TextMeshProUGUI jigsawPuzzleCountdownTimerText;
    public Button jigsawPuzzleInteractButton;
    public TextMeshProUGUI jigsawPuzzleInteractText;

    [Header("Spot the Differences Puzzle")]
    public CanvasGroup differencePuzzleCanvasGroup;
    public TextMeshProUGUI numberOfDifferencesFoundText;
    public TextMeshProUGUI diffPuzzleCountdownTimerText;
    public Button diffPuzzleInteractButton;
    public TextMeshProUGUI diffPuzzleInteractText;

    [Header("Text Popup")]
    public TextMeshProUGUI popupTextUI;
}
