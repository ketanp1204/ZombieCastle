using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultipleZombiesDeathBehaviour : MonoBehaviour
{
    // Singleton
    public static MultipleZombiesDeathBehaviour instance;

    // Public variables
    public UnityEvent allZombiesDead;
    [TextArea(3,6)]
    public string[] dialogueAfterCombat;

    // Private variables
    private int numberOfZombies;
    private int currentNumberOfDeadZombies = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        numberOfZombies = transform.childCount;
    }

    public void AddDeadZombie()
    {
        currentNumberOfDeadZombies += 1;

        if (currentNumberOfDeadZombies == numberOfZombies)
        {
            allZombiesDead.Invoke();
            Player.SetIdleState();
        }
    }

    public void ShowDialogueAfterCombat()
    {
        Player.StopMovement();
        new Task(ShowDialogueAfterDelay());   
    }

    private IEnumerator ShowDialogueAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (DialogueBox.instance)
        {
            DialogueBox.instance.FillSentences(dialogueAfterCombat);
            DialogueBox.instance.StartDialogueDisplay();
        }
    }

    public void SetRoom1CombatCompletedBool()
    {
        GameData.r1_combatCompleted = true;
    }

    public void SetRoom2CombatCompletedBool()
    {
        GameData.r2_combatCompleted = true;
    }

    public void SetRoom5CombatCompleteBool()
    {
        GameData.r5_zombie3CombatCompleted = true;
    }
}
