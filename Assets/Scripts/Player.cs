using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerCombat))]

public class Player : MonoBehaviour
{
    // Singleton
    public static Player instance;

    // Private Cached References
    private PlayerInput playerInput;                // Reference To The PlayerInput Class
    private PlayerCombat playerCombat;              // Reference To The PlayerCombat Class
    private Rigidbody2D rb;                         // Reference To The Player's Rigidbody Component
    private Animator animator;                      // Reference To The Player's Animator Component
    private HealthBar healthBar;                    // Reference To The Player's Health Bar 
    private UnityEngine.Object playerReference;

    // Public Cached References
    public Transform pathfindingTarget;             // Reference to the player's pathfinding target


    // Variables : Player
    [HideInInspector]
    public bool movePlayer;                         // Bool to store whether Player can move
    [HideInInspector]
    public bool weaponDrawn;                        // Bool to store whether weapon is drawn
    [HideInInspector]
    public bool IsDead = false;                     // Bool to store if player dies
    private bool facingRight;                       // Player's Direction Of Facing
    [HideInInspector]
    public bool isClimbingLadder = false;           // Bool to store whether player is climbing a ladder
    [HideInInspector]
    public float movementSpeed;                     // Player's Current Movement Speed;
    public float walkSpeed;                         // Player's Walking Speed
    public float walkFastSpeed;                     // Player's Running Speed
    private Vector2 movement;                       // Player's Movement Vector
    public int maxHealth = 100;                     // Player's Maximum Health
    [HideInInspector]
    public int currentHealth;                       // Player's current health
    [HideInInspector]
    public bool takingDamage;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        SetReferences();
        InitializeValues();

        // Ignore player collisions with dead zombies
        Physics2D.IgnoreLayerCollision(gameObject.layer, 13);
    }

    void OnEnable()
    {
        playerReference = Resources.Load(gameObject.name);
    }

    // Link Cached References
    void SetReferences()
    {
        playerInput = GetComponent<PlayerInput>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
    }

    private void InitializeValues()
    {
        facingRight = true;
        movePlayer = true;
        takingDamage = false;
        weaponDrawn = true;             // FOR TESTING. CHANGE LATER.
        movementSpeed = walkSpeed;
        animator.SetFloat("FaceDir", 1f);

        if (PlayerStats.isFirstScene)
        {
            currentHealth = maxHealth;
            PlayerStats.currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            PlayerStats.isFirstScene = false;
        }
        else
        {
            currentHealth = PlayerStats.currentHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    void FixedUpdate()
    {
        // Moving the player
        if (movePlayer)
        {
            HandleMovement();

            // Flip the player direction depending on where he is facing
            FlipPlayerDirection();
        }

        // Player attacks enemies
        HandleAttacks();

        // Reset parameters
        ResetValues();
    }

    private void HandleMovement()
    {
        float horizontal, vertical;

        horizontal = playerInput.horizontalInput;
        bool walkFast = playerInput.walkFastInput;

        movement.x = horizontal;
        movement.y = rb.velocity.y;

        if(isClimbingLadder)
        {
            vertical = playerInput.verticalInput;
            movement.y = vertical;
        }

        movement.Normalize();

        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
        {
            if (!takingDamage)
            {
                rb.MovePosition((Vector2)transform.position + (movement * movementSpeed * Time.deltaTime));
            }
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        if (walkFast == true)
        {
            animator.SetBool("WalkFast", true);
            movementSpeed = walkFastSpeed;
        }

        if (walkFast == false)
        {
            animator.SetBool("WalkFast", false);
            movementSpeed = walkSpeed;
        }
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

            /*
            // Multiply the scale of the player object to flip the sprite
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            */
        }
    }

    private void HandleAttacks()
    {
        if(weaponDrawn)
        {
            if (playerInput.attack1Pressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1"))
            {
                playerCombat.InvokeAttack1();
                animator.SetTrigger("Attack1");
                rb.velocity = Vector2.zero;
            }

            if (playerInput.attack2Pressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
            {
                playerCombat.InvokeAttack2();
                animator.SetTrigger("Attack2");
                rb.velocity = Vector2.zero;
            }

            /*
            if (playerInput.attack1Released && this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1"))
            {
                animator.SetBool("Attack1", false);
            }

            if (playerInput.attack2Released && this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
            {
                animator.SetBool("Attack2", false);
            }
            */
        }
    }

    private void ResetValues()
    {
        
    }

    public static bool PlayerFacingRight()
    {
        return instance.facingRight;
    }

    // Sets the WeaponDrawn animation parameter to true
    public static void DrawWeapon()
    {
        instance.animator.SetBool("WeaponDrawn", true);
    }

    // Sets the WeaponDrawn animation parameter to false
    public static void RemoveWeapon()
    {
        instance.animator.SetBool("WeaponDrawn", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // rb.velocity = Vector2.zero;
            // animator.SetFloat("Magnitude", 0f);
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
