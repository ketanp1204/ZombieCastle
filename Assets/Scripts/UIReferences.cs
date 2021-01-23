using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIReferences : MonoBehaviour
{
    [Header("General")]
    public Camera mainCamera;
    public GameObject pauseMenuUI;

    [Header("Canvases")]
    public Canvas staticUICanvas;
    public Canvas dynamicUICanvas;
    
    [Header("Dialogue System")]
    public GameObject dialogueManager;
    public Animator dialogueBoxAnimator;                                    
    public TextMeshProUGUI dialogueText;                                    
    public GameObject dBoxContinueButton;                                   
    public Animator noteAnimator;                                           
    public TextMeshProUGUI noteText;                                        
    public GameObject noteContinueButton;                                   

}
