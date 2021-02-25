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
    public LayerMask groundLayerMask;

    // Public Cached References
    public Transform pathfindingTarget;             // Reference to the player's pathfinding target

    // Variables : Player
    [HideInInspector]
    public bool movePlayer;                         // Bool to store whether Player can move
    [HideInInspector]
    public bool axeDrawn;                           // Bool to store whether axe is drawn
    [HideInInspector]
    public bool knifeDrawn;                           // Bool to store whether knife is drawn
    private bool facingRight;                       // Player's direction of facing
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

        // Ignore player collision with maze collider
        Physics2D.IgnoreLayerCollision(gameObject.layer, 14);
    }

    void OnEnable()
    {
        playerReference = Resources.Load(gameObject.name);
        ResetAnimatorParameters();
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
        facingRight = false;
        movePlayer = true;
        takingDamage = false;
        axeDrawn = false;             
        knifeDrawn = true;
        animator.SetBool("KnifeDrawn", true);       // FOR TESTING, CHANGE LATER.
        movementSpeed = walkSpeed;
        animator.SetFloat("FaceDir", -1f);

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

    private void ResetAnimatorParameters()
    {
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Magnitude", 0f);
        animator.SetFloat("FaceDir", -1f);
        animator.SetBool("WalkFast", false);
        animator.SetBool("IsDead", false);
        animator.SetBool("AxeDrawn", false);
        animator.SetBool("KnifeDrawn", true);
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
        movement.y = 0f;

        if (isClimbingLadder)
        {
            vertical = playerInput.verticalInput;
            movement.y = vertical;
        }


        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            if (!takingDamage)
            {
                movement.Normalize();
                rb.velocity = new Vector2(movement.x * movementSpeed * Time.deltaTime, rb.velocity.y);
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
        if(axeDrawn)
        {
            if (playerInput.leftMousePressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt") && !PlayerStats.IsDead)
            {
                playerCombat.InvokeAxeAttack();
                animator.SetTrigger("AxeAttack");
                rb.velocity = Vector2.zero;
            }
        }

        if (knifeDrawn)
        {
            if (playerInput.leftMousePressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt") && !PlayerStats.IsDead)
            {
                playerCombat.InvokeKnifeAttack();
                animator.SetTrigger("KnifeAttack");
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void ResetValues()
    {
        
    }

    public static bool PlayerFacingRight()
    {
        return instance.facingRight;
    }

    public static void StopMovement()
    {
        instance.movePlayer = false;
    }

    public static void EnableMovement()
    {
        instance.movePlayer = true;
    }

    // Sets the AxeDrawn animation parameter to true
    public static void EquipAxe()
    {
        instance.axeDrawn = true;
        instance.animator.SetBool("AxeDrawn", true);

        // Set other weapon bools to false
        instance.knifeDrawn = false;
        instance.animator.SetBool("KnifeDrawn", false);
    }

    // Sets the AxeDrawn animation parameter to false
    public static void UnequipAxe()
    {
        instance.axeDrawn = false;
        instance.animator.SetBool("AxeDrawn", false);
    }

    // Sets the KnifeDrawn animation parameter to true
    public static void EquipKnife()
    {
        instance.knifeDrawn = true;
        instance.animator.SetBool("KnifeDrawn", true);

        // Set other weapon bools to false
        instance.axeDrawn = false;
        instance.animator.SetBool("AxeDrawn", false);
    }

    // Sets the KnifeDrawn animation parameter to false
    public static void UnequipKnife()
    {
        instance.knifeDrawn = false;
        instance.animator.SetBool("KnifeDrawn", false);
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
