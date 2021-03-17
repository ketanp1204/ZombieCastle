using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeExitDoor : MonoBehaviour
{
    // Instance
    public static MazeExitDoor instance;

    // Private Cached References
    private BoxCollider2D boxCollider;
    private SpriteRenderer doorSR;

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
        boxCollider = GetComponent<BoxCollider2D>();
        doorSR = GetComponent<SpriteRenderer>();
    }

    public void EnableExit()
    {
        boxCollider.isTrigger = true;

        doorSR.enabled = false;
    }

    public void EnableDoorSprite()
    {
        doorSR.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MazePuzzle.instance.StopTimerOnPuzzleSuccess();
    }
}
