using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayGameInstructions : MonoBehaviour
{
    // Instance
    public static DisplayGameInstructions instance;

    // Private References
    private UIReferences uiReferences;
    private TextMeshProUGUI popupTextUI;

    // Instruction Texts
    [TextArea(2,5)]
    public string movementInstructionText;
    [TextArea(2, 5)]
    public string toolbarInstructionText;
    [TextArea(2, 5)]
    public string inventoryBoxInstructionText;
    [TextArea(2, 5)]
    public string objectAndDoorInteractionInstructionText;

    // Public Variables

    // HasSeen bools - player has seen a particular instruction
    [HideInInspector]
    public bool hasSeenMovementInstruction = false;
    [HideInInspector]
    public bool hasSeenInteractionInstruction = false;
    [HideInInspector]
    public bool hasSeenInventoryBoxInstruction = false;
    [HideInInspector]
    public bool hasSeenToolbarInstruction = false;

    // CanShow bools - to enable display of a particular instruction
    public bool canShowMovementInstruction;
    public bool canShowInteractionInstruction;
    public bool canShowInventoryBoxInstruction;
    public bool canShowToolbarInstruction;

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
        uiReferences = GameSession.instance.uiReferences;
        popupTextUI = uiReferences.popupTextUI;
    }

    public void UnsetCanShowBools()
    {
        canShowMovementInstruction = false;
        canShowInteractionInstruction = false;
        canShowToolbarInstruction = false;
        canShowInventoryBoxInstruction = false;
    }

    /// <summary>
    /// Set CanShow bools to true
    /// </summary>
    public void SetCanShowMovementInstruction()
    {
        canShowMovementInstruction = true;
    }

    public void SetCanShowInteractionInstruction()
    {
        canShowInteractionInstruction = true;
    }

    public void SetCanShowToolbarInstruction()
    {
        canShowToolbarInstruction = true;
    }

    /// <summary>
    /// Set CanShow bools to false
    /// </summary>
    public void SetCanShowInventoryBoxInstruction()
    {
        canShowInventoryBoxInstruction = true;
    }

    public void UnsetCanShowMovementInstruction()
    {
        canShowMovementInstruction = false;
    }

    public void UnsetCanShowInteractionInstruction()
    {
        canShowInteractionInstruction = false;
    }

    public void UnsetCanShowToolbarInstruction()
    {
        canShowToolbarInstruction = false;
    }

    public void UnsetCanShowInventoryBoxInstruction()
    {
        canShowInventoryBoxInstruction = false;
    }

    /// <summary>
    /// Start displaying instructions
    /// </summary>
    public void StartInstructionsDisplay()
    {
        new Task(DisplayInstructions());
    }

    private IEnumerator DisplayInstructions()
    {
        // Disable player selection
        if (PlayerTopDown.instance)
        {
            PlayerTopDown.instance.DisableSelectionCollider();
        }
        else
        {
            Player.instance.DisableSelectionCollider();
        }

        yield return new WaitForSeconds(2f);

        // Show movement instruction
        if (!hasSeenMovementInstruction && canShowMovementInstruction)
        {
            popupTextUI.text = movementInstructionText;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

            // Hide instruction text
            yield return new WaitForSeconds(4f);
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
            yield return new WaitForSeconds(0.6f);

            hasSeenMovementInstruction = true;
        }

        // Show object interaction instructions
        if (!hasSeenInteractionInstruction && canShowInteractionInstruction)
        {
            popupTextUI.text = objectAndDoorInteractionInstructionText;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

            // Hide instruction text
            yield return new WaitForSeconds(4f);
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
            yield return new WaitForSeconds(0.6f);

            hasSeenInteractionInstruction = true;
        }

        // Show toolbar instruction
        if (!hasSeenToolbarInstruction && canShowToolbarInstruction)
        {
            popupTextUI.text = toolbarInstructionText;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

            // Hide instruction text
            yield return new WaitForSeconds(4f);
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
            yield return new WaitForSeconds(0.6f);

            hasSeenToolbarInstruction = true;
        }

        // Show inventory box instruction
        if (!hasSeenInventoryBoxInstruction && canShowInventoryBoxInstruction)
        {
            yield return new WaitForSeconds(2f);

            popupTextUI.text = inventoryBoxInstructionText;
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

            // Hide instruction text
            yield return new WaitForSeconds(4f);
            new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
            yield return new WaitForSeconds(0.6f);

            hasSeenInventoryBoxInstruction = true;
        }
        
        // Enable player selection
        if (PlayerTopDown.instance)
        {
            PlayerTopDown.instance.EnableSelectionCollider();
        }
        else
        {
            Player.instance.EnableSelectionCollider();
        }
    }
}
