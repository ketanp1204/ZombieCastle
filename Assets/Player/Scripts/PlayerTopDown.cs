using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerTopDown : MonoBehaviour
{
    // Singleton
    public static PlayerTopDown instance;


    // Private References
    private UIReferences uiReferences;                                              // Reference - Current UIReferences script in the scene

    private PlayerInput playerInput;                                                // Reference - PlayerInput class

    private Rigidbody2D rb;                                                         // Reference - Rigidbody component

    private Animator animator;                                                      // Reference - Animator component

    private HealthBar healthBar;                                                    // Reference - Health bar 


    // Public References
    public GameObject selectionArea;                                                // Reference - Player selection area - Used to prevent selection during game instructions display


    // Public Variables
    [HideInInspector]
    public bool movePlayer;                                                         // Bool - Player can move

    [HideInInspector]
    public float movementSpeed;                                                     // Float - Current movement speed;

    [HideInInspector]
    public int currentHealth;                                                       // Int - Current health

    public float walkSpeed;                                                         // Float - Walk speed

    public int maxHealth = 100;                                                     // Int - Maximum health
    
    // Private Variables
    private bool facingRight;                                                       // Bool - Direction of facing

    private Vector2 movement;                                                       // Vector2 - Movement vector

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        SetReferences();
        Initialize();
        ResetAnimatorMovementParameters();

        if (PlayerStats.isFirstScene)
        {
            animator.SetBool("IsFirstScene", true);
            if (!GameData.lobby_introDialogueSeen)
                StopMovement();
        }
        else
        {
            animator.SetBool("IsFirstScene", false);
            HandleSceneChanges();
        }
    }

    // Store References
    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        healthBar = uiReferences.playerHealthBar;
        
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Default Values
    public void Initialize()
    {
        // Movement
        facingRight = false;
        movePlayer = true;
        movementSpeed = walkSpeed;
        animator.SetFloat("FaceDir", -1f);

        // Health
        if (PlayerStats.isFirstScene)
        {
            currentHealth = maxHealth;
            PlayerStats.currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }
        else
        {
            currentHealth = PlayerStats.currentHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    private void ResetAnimatorMovementParameters()
    {
        animator.SetFloat("Horizontal", 0f);        // No keyboard input
        animator.SetFloat("Vertical", 0f);          // No keyboard input
        animator.SetFloat("Magnitude", 0f);         // No keyboard input
        animator.SetFloat("FaceDir", -1f);          // Facing left
    }

    private void HandleSceneChanges()
    {
        if (GameData.sceneName == LevelManager.SceneNames.Room1)
        {
            transform.position = new Vector3(-5.34f, -3.91f, 0f);
        }
        else if (GameData.sceneName == LevelManager.SceneNames.Room2)
        {
            transform.position = new Vector3(-2.71f, -1.88f, 0f);
        }
        else if (GameData.sceneName == LevelManager.SceneNames.Room3)
        {
            transform.position = new Vector3(2.84f, -2.01f, 0f);
        }

        GameData.sceneName = LevelManager.SceneNames.Lobby;
    }

    public void DisableSelectionCollider()
    {
        selectionArea.SetActive(false);
    }

    public void EnableSelectionCollider()
    {
        selectionArea.SetActive(true);
    }

    void FixedUpdate()
    {
        // Player movement
        if (movePlayer)
        {
            HandleMovement();

            // Flip the player direction depending on where he is facing
            FlipPlayerDirection();
        }
    }

    private void HandleMovement()
    {
        float horizontal = playerInput.horizontalInput;
        float vertical = playerInput.verticalInput;

        movement.x = horizontal;
        movement.y = vertical;
        movement.Normalize();

        rb.MovePosition((Vector2)transform.position + (movement * movementSpeed * Time.deltaTime));

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);
    }

    private void FlipPlayerDirection()
    {
        float horizontal = movement.x;
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            if (facingRight)
            {
                // Set animation parameter
                animator.SetFloat("FaceDir", 1f);
            }
            else
            {
                // Set animation parameter
                animator.SetFloat("FaceDir", -1f);
            }
        }
    }

    public void PlayFootStepSound()
    {
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerFootStep);
    }

    public void PlayLowHealthBreathingSound()
    {
        if (PlayerStats.currentHealth < 30)
        {
            AudioManager.PlayPersistentSingleSoundIfNotAlreadyPlaying(AudioManager.Sound.PlayerLowHealthBreathing);
        }
    }

    public static bool PlayerFacingRight()
    {
        if (instance != null)
        {
            return instance.facingRight;
        }
        else
            return false;
    }

    // Stop input for movement
    public static void StopMovement()
    {
        if (instance != null)
        {
            instance.movePlayer = false;

            // Reset movement animation parameters
            instance.animator.SetFloat("Horizontal", 0f);
            instance.animator.SetFloat("Vertical", 0f);
            instance.animator.SetFloat("Magnitude", 0f);
        }
    }

    // Enable input for movement
    public static void EnableMovement()
    {
        if (instance != null)
        {
            instance.movePlayer = true;
        }
    }
}
