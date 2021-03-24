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
    public string interactionInstructionText;
    [TextArea(2, 5)]
    public string moreControlsRedirectText;

    void Awake()
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

        // Disable toolbar and inventory open
        ToolbarManager.DisableToolbarOpen();
        InventoryManager.DisableInventoryOpen();

        yield return new WaitForSeconds(2f);


        // Show movement instruction
        popupTextUI.text = movementInstructionText;
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

        // Hide instruction text
        yield return new WaitForSeconds(4f);
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
        yield return new WaitForSeconds(0.6f);



        // Show interaction instruction
        popupTextUI.text = interactionInstructionText;
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

        // Hide instruction text
        yield return new WaitForSeconds(4f);
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
        yield return new WaitForSeconds(0.6f);



        // Show toolbar instruction
        popupTextUI.text = toolbarInstructionText;
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

        // Allow inventory and toolbar to open 
        ToolbarManager.EnableToolbarOpen();
        InventoryManager.EnableInventoryOpen();

        // Hide instruction text
        yield return new WaitForSeconds(4f);
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
        yield return new WaitForSeconds(0.6f);



        // Show inventory box instruction
        popupTextUI.text = inventoryBoxInstructionText;
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

        // Hide instruction text
        yield return new WaitForSeconds(4f);
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
        yield return new WaitForSeconds(0.6f);



        // Show more controls redirect instruction
        popupTextUI.text = moreControlsRedirectText;
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

        // Hide instruction text
        yield return new WaitForSeconds(4f);
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
        yield return new WaitForSeconds(0.6f);




        // Enable player selection
        if (PlayerTopDown.instance)
        {
            PlayerTopDown.instance.EnableSelectionCollider();
        }
        else
        {
            Player.instance.EnableSelectionCollider();
        }

        Destroy(gameObject);
    }
}
