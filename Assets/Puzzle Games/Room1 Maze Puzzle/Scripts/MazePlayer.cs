using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayer : MonoBehaviour
{
    // Instance
    public static MazePlayer instance;

    // Public variables
    public float moveSpeed;
    [HideInInspector]
    public bool movePlayer;                                             // Bool - Player can move

    // Private variables
    private Vector2 movement;
    private float startPositionX = -6.6f;                              // From inspector
    private float startPositionY = -3.279f;                              // From inspector

    // Private Cached References
    private PlayerInput playerInput;                                    // Reference To The PlayerInput Class
    private Rigidbody2D rb;                                             // Reference To The Player's Rigidbody Component

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
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        movePlayer = false;
    }

    public static void StartPuzzle()
    {
        instance.movePlayer = true;
    }

    public static void EndPuzzle()
    {
        SetStartPosition();
        instance.movePlayer = false;
    }

    public static void SetStartPosition()
    {
        instance.transform.position = new Vector3(instance.startPositionX, instance.startPositionY, 0f);
    }

    void FixedUpdate()
    {
        if (movePlayer)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        float horizontal, vertical;

        horizontal = playerInput.horizontalInput;
        vertical = playerInput.verticalInput;

        movement.x = horizontal;
        movement.y = vertical;
        movement.Normalize();

        rb.MovePosition((Vector2)transform.position + (movement * moveSpeed * Time.deltaTime));
    }
}
