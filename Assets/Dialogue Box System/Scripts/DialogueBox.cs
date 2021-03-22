using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    // Singleton
    public static DialogueBox instance;

    // Private References
    private CanvasGroup dialogueBoxCG;

    // Public References
    public TextMeshProUGUI dialogueText;
    public GameObject continueButton;

    // Public variables
    [HideInInspector]
    public bool isTyping = false;                                       // Bool - Dialogue box active and typing status

    public float typingSpeed = 0.025f;                                  // Float - Text auto-type speed in seconds
    public float dialogueDisplayDelay = 0.2f;                           // Float - Delay time before displaying the dialogue box
    public float textSoundEffectPlayInterval = 0.09f;                   // Float - Interval between each text type sound effect

    // Private variables
    private string[] sentences;                                         // String array - Sentence array to be displayed
    private bool skipAutoTyping = false;                                // Bool - To stop auto typing of current dialogue box text
    private bool skippedCurrentDialogue = false;                        // Bool - Player skipped current auto-typing dialogue
    private int dialogueIndex = 0;                                      // Int -  Current sentence index
    private float textTypingDelay;                                      // Float - Delay time before starting auto typing of text
    private bool continueButtonEnabled = false;                         // Bool - If enabled, player can press space bar to press the continue button

    // For events after dialogue box display
    private bool addToInventoryAfterDialogue = false;                   // Bool - Object goes to inventory after dialogue display
    private bool showNoteAfterDisplay = false;                          // Bool - Object has note box display after dialogue display
    private bool room1MazePuzzleAfterDialogue = false;                  // Bool - Portrait object in room1 that contains maze puzzle
    private bool room3DiffPuzzleAfterDialogue = false;                  // Bool - Cupboard object in room3 that contains differences puzzle
    private bool room1ZombieDiscovery = false;                          // Bool - Player finds a zombie in room1
    private bool isGameStartDialogue = false;                           // Bool - To start instructions display after game start dialogue

    private ItemObject currentItem = null;                              // ItemObject - The current ItemObject which has the player comment response

    private GameObject pcThenInventoryGO = null;                        // GameObject - For PC_Then_Inventory objects, this will be used to remove the object from the scene after the dialogue is over

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        dialogueBoxCG = GetComponent<CanvasGroup>();
    }

    private void Initialize()
    {
        // Hide dialogue box on start
        dialogueBoxCG.alpha = 0f;
        dialogueBoxCG.interactable = false;
        dialogueBoxCG.blocksRaycasts = false;
    }

    public void FillSentences(string[] _sentences)
    {
        sentences = _sentences;
    }

    public void SetCurrentItem(ItemObject item)
    {
        currentItem = item;
    }

    public void StartDialogueDisplay()
    {
        Player.StopMovement();
        PlayerTopDown.StopMovement();

        // Fade in dialogue box
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(dialogueBoxCG, 0f, 1f, dialogueDisplayDelay));
        dialogueBoxCG.interactable = true;
        dialogueBoxCG.blocksRaycasts = true;

        textTypingDelay = dialogueDisplayDelay + 0.2f;

        StartCoroutine(TypeText(textTypingDelay));
    }

    private IEnumerator TypeText(float delay)                                                       // Starts typing text after delay
    {
        yield return new WaitForSeconds(delay);

        isTyping = true;

        new Task(PlayTextTypeSoundEffect());

        foreach (char letter in sentences[dialogueIndex].ToCharArray())
        {
            if (skipAutoTyping)
            {
                dialogueText.text = sentences[dialogueIndex];
                skipAutoTyping = false;
                break;
            }
            else
            {
                dialogueText.text += letter;                                                        // Auto-typing text              
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
    }

    private IEnumerator PlayTextTypeSoundEffect()
    {
        while (isTyping)
        {
            AudioManager.PlayOneShotSound(AudioManager.Sound.TextAutoTypingSound);
            yield return new WaitForSeconds(textSoundEffectPlayInterval);
        }
    }

    public void NextDialogueSentence()
    {
        // Play continue button sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.ContinueButton);

        continueButton.SetActive(false);                                                            // Hide dialogue box continue button
        continueButtonEnabled = false;

        if (dialogueIndex < sentences.Length - 1)                                                   // More dialogue texts left to display
        {
            dialogueIndex++;
            dialogueText.text = "";
            StartCoroutine(TypeText(0f));                                                           // No delay in between dialogue sentences
        }
        else
        {                                                                                           
            dialogueText.text = "";                                                                 // No more dialogue texts left to display

            Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock cursor
            isTyping = false;                                                                       // Text is not being typed in the dialogue box

            // Fade out the dialogue box
            new Task(UIAnimation.FadeCanvasGroupAfterDelay(dialogueBoxCG, 1f, 0f, 0f));

            if (addToInventoryAfterDialogue)
            {
                // TODO: Open inventory box or flash inventory bag in toolbar (preferably latter) to indicate newly added item
                if (InventoryManager.instance)
                {
                    InventoryManager.instance.AddInventoryItem(currentItem);
                }

                // Remove the object from the scene
                if (pcThenInventoryGO != null)
                {
                    Destroy(pcThenInventoryGO);
                }

            }

            if (showNoteAfterDisplay)
            {
                NoteBox.instance.SetCurrentItem(currentItem);
                NoteBox.instance.FillNotePageText();
                NoteBox.instance.ShowNoteBox();
            }

            if (room1MazePuzzleAfterDialogue)
            {
                // Load maze puzzle UI
                MazePuzzle.LoadMazePuzzleUI();
            }

            if (room3DiffPuzzleAfterDialogue)
            {
                // Load diff puzzle UI
                SpotDifferencesPuzzle.LoadDiffPuzzleUI();
            }

            if (room1ZombieDiscovery)
            {
                R1_CombatStartBehaviour.instance.ShowInventoryAfterKnifeOpenDialogue();
            }

            if (isGameStartDialogue)
            {
                if (DisplayGameInstructions.instance)
                {
                    DisplayGameInstructions.instance.StartInstructionsDisplay();
                }
            }

            Player.EnableMovement();
            PlayerTopDown.EnableMovement();

            ResetValues();
        }
    }

    public void SetPCThenInventoryGameObject(GameObject pcThenInv)
    {
        pcThenInventoryGO = pcThenInv;
    }

    public void SetInventoryAfterDialogueFlag()
    {
        addToInventoryAfterDialogue = true;
    }

    public void SetNoteAfterDialogueFlag()
    {
        showNoteAfterDisplay = true;
    }

    public void SetMazePuzzleFlag()
    {
        room1MazePuzzleAfterDialogue = true;
    }

    public void SetDiffPuzzleFlag()
    {
        room3DiffPuzzleAfterDialogue = true;
    }

    public void SetRoom1ZombieDiscoveryFlag()
    {
        room1ZombieDiscovery = true;
    }

    public void SetGameStartDialogueFlag()
    {
        isGameStartDialogue = true;
    }

    private void ResetValues()
    {
        dialogueIndex = 0;

        isTyping = false;

        skipAutoTyping = false;
        skippedCurrentDialogue = false;

        dialogueBoxCG.interactable = false;                                                         // Prevent UI elements from being clickable
        dialogueBoxCG.blocksRaycasts = false;

        dialogueDisplayDelay = 0.2f;

        // Reset event bools
        addToInventoryAfterDialogue = false;
        showNoteAfterDisplay = false;
        room1MazePuzzleAfterDialogue = false;
        room3DiffPuzzleAfterDialogue = false;
        room1ZombieDiscovery = false;
        isGameStartDialogue = false;

        currentItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTyping)
        {
            if (dialogueText.text == sentences[dialogueIndex])
            {
                continueButton.SetActive(true);                                                     // Show dialogue box continue button after auto-typing is finished
                new Task(SetContinueButtonEnabledFlagToTrueAfterDelay());

                Cursor.lockState = CursorLockMode.Locked;                                           // Center and lock mouse cursor
                Cursor.lockState = CursorLockMode.None;                                             // Unlock mouse cursor
            }

            if (Input.GetKeyDown(KeyCode.Space) && !skippedCurrentDialogue)
            {
                continueButton.SetActive(true);                                                     // Show dialogue box continue button if player skips the dialogue by pressing space
                new Task(SetContinueButtonEnabledFlagToTrueAfterDelay());

                Cursor.lockState = CursorLockMode.Locked;                                           // Center and lock mouse cursor
                Cursor.lockState = CursorLockMode.None;                                             // Unlock mouse cursor

                skipAutoTyping = true;
                skippedCurrentDialogue = true;
            }
        }

        if (continueButtonEnabled)
        {
            // Click continue button by pressing space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                continueButton.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    private IEnumerator SetContinueButtonEnabledFlagToTrueAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);

        continueButtonEnabled = true;
    }
}
