using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSwitch : MonoBehaviour
{
    public enum SwitchIndex
    {
        First,
        Second
    }

    // Public Variables
    public SwitchIndex switchIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Disable collider
        GetComponent<BoxCollider2D>().enabled = false;

        // Switch Sprite to closed switch
        MazePuzzle.instance.ChangeSwitchSpriteToClosed(switchIndex);

        // Start puzzle timer if this collision was the first, otherwise add bonus time
        if (MazePuzzle.instance.timerStarted)
            MazePuzzle.instance.AddBonusTime();
        else
            MazePuzzle.instance.StartPuzzleTimer();
    }
}
