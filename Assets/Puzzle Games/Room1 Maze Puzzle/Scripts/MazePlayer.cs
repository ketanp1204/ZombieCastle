﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayer : MonoBehaviour
{
    // Instance
    public static MazePlayer instance;

    public float moveSpeed;
    [HideInInspector]
    public bool movePlayer;                         // Bool - Player can move
    private Vector2 movement;

    // Private Cached References
    private PlayerInput playerInput;                // Reference To The PlayerInput Class
    private Rigidbody2D rb;                         // Reference To The Player's Rigidbody Component

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
        Player.StopMovement();
        instance.StartCoroutine(instance.EnableMovementAfterDelay(1.5f));
    }

    public static void EndPuzzle()
    {
        instance.movePlayer = false;
        Player.EnableMovement();
    }

    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        movePlayer = true;
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
