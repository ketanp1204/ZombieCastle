using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzleSolved : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        portrait_maze_puzzle.instance.ClosePuzzle();
    }
}
