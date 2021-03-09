using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuBehavior : MonoBehaviour
{

    // Cached References
    public Animator animator;           

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor
    }

    public void StartGame()
    {
        animator.SetTrigger("FadeOut");
        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
        LevelManager.LoadNextLevel();
    }
}
