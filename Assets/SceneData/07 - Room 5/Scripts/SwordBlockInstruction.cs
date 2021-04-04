using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwordBlockInstruction : MonoBehaviour
{
    // Private variables
    private UIReferences uiReferences;
    private TextMeshProUGUI popupTextUI;

    // Instruction Text
    [TextArea(2, 5)]
    public string swordBlockInstructionText;

    // Start is called before the first frame update
    void Start()
    {
        uiReferences = GameSession.instance.uiReferences;
        popupTextUI = uiReferences.popupTextUI;

        if (!GameData.r5_instructionSeen)
        {
            GameData.r5_instructionSeen = true;
            new Task(DisplaySwordBlockInstruction());
        }
    }

    private IEnumerator DisplaySwordBlockInstruction()
    {
        yield return new WaitForSeconds(3f);

        // Show instruction
        popupTextUI.text = swordBlockInstructionText;
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 0f, 1f, 0f));

        // Hide instruction text
        yield return new WaitForSeconds(4f);
        new Task(UIAnimation.FadeTMProTextAfterDelay(popupTextUI, 1f, 0f, 0f));
        yield return new WaitForSeconds(0.6f);
    }
}
