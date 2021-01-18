using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReferences : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject pauseMenuUI;
    public Canvas staticUICanvas;
    public Canvas dynamicUICanvas;

    [HideInInspector]
    public GameObject gameSession;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = GameObject.Find("GameSession");
    }
}
