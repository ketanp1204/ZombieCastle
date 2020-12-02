using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToNextScene : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LevelManager.LoadNextLevel();
    }
}
