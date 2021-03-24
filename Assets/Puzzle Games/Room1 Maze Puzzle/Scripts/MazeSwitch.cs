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

        // Play switch enable sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.MazeSwitchEnable);

        // Switch Sprite to enabled switch
        MazePuzzle.instance.ChangeSwitchSpriteToOn(switchIndex);

        // Add bonus time
        MazePuzzle.instance.AddBonusTime();

        // Set first switch hit flag on puzzle script
        if (!MazePuzzle.instance.firstSwitchHitFlag)
            MazePuzzle.instance.firstSwitchHitFlag = true;
    }
}
