using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class portrait_maze_puzzle : MonoBehaviour
{
    public static portrait_maze_puzzle instance;

    // Private Cached References
    private UIReferences uiReferences;

    // Public Cached References
    public SpriteRenderer e_key_SR;
    public GameObject mazePuzzleGO;
    public CinemachineVirtualCamera cam;

    // Private Variables
    private bool isActive;
    private bool checkForInput = false;
    private float mazePuzzleDisplayDelay = 0.3f;

    private GameObject mazeCollidersGO;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        mazePuzzleGO.SetActive(false);

        uiReferences = GameSession.instance.uiReferences;
        uiReferences.mazeCloseButtonCanvasGroup.interactable = false;
        uiReferences.mazeCloseButtonCanvasGroup.blocksRaycasts = false;

        isActive = false;

        e_key_SR = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        e_key_SR.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            e_key_SR.enabled = true;
            checkForInput = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            e_key_SR.enabled = false;
            checkForInput = false;
        }
    }

    void Update()
    {
        if (checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isActive)
            {
                // maze is active
                isActive = true;

                mazePuzzleGO.SetActive(true);

                cam.Follow = mazePuzzleGO.transform;

                // show maze close button
                uiReferences.mazeCloseButtonCanvasGroup.interactable = true;
                uiReferences.mazeCloseButtonCanvasGroup.blocksRaycasts = true;
                new Task(UIAnimation.FadeCanvasGroupAfterDelay(uiReferences.mazeCloseButtonCanvasGroup, 0f, 1f, mazePuzzleDisplayDelay));

                // Stop Player mvovement
                Player.StopMovement();

                mazePlayer.StartPuzzle();

                Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor
                Cursor.lockState = CursorLockMode.None;                                                 // Unlock mouse cursor
            }
        }
    }

    public void ClosePuzzle()
    {
        mazePuzzleGO.SetActive(false);

        // Fade out the maze close button
        uiReferences.mazeCloseButtonCanvasGroup.interactable = false;
        uiReferences.mazeCloseButtonCanvasGroup.blocksRaycasts = false;
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(uiReferences.mazeCloseButtonCanvasGroup, uiReferences.mazeCloseButtonCanvasGroup.alpha, 0f, 0f));

        // Enable Player movement
        Player.EnableMovement();

        mazePlayer.EndPuzzle();

        // maze is inactive
        isActive = false;

        Cursor.lockState = CursorLockMode.Locked;                                               // Center and lock mouse cursor

        cam.Follow = FindObjectOfType<Player>().transform;
    }

    public static bool IsActive()
    {
        return instance.isActive;
    }
}
