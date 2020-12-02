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
        Cursor.visible = true;
    }

    public void StartGame()
    {
        animator.SetTrigger("FadeOut");
        Cursor.visible = false;
        LevelManager.LoadNextLevel();
    }
}
