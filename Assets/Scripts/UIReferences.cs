using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReferences : MonoBehaviour
{

    public GameObject pauseMenuUI;

    [HideInInspector]
    public GameObject gameSession;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = GameObject.Find("GameSession");
    }
}
