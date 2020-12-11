using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    // Singleton
    public static Player instance;

    // Cached References
    private PlayerInput playerInput;                // Reference To The PlayerInput Class
    private Rigidbody2D rb;                         // Reference To The Player's Rigidbody Component
    private Animator animator;                      // Reference To The Player's Animator Component
    private HealthBar healthBar;                    // Reference To The Player's Health Bar 
    private SceneType sceneType;                    // Reference To The Current Type Of Scene: 2D or 2.5D
    private SpriteRenderer sR;                      // Reference To The Player GameObject's SpriteRenderer

    // Variables : Player
    public float movementSpeed;                     // Player's Current Movement Speed;
    public float wSpeed;                            // Player's Walking Speed
    public float rSpeed;                            // Player's Running Speed
    private bool facingRight;                       // Player's Direction Of Facing
    private Vector2 movement;                       // Player's Movement Vector
    public int maxHealth = 100;                     // Player's Maximum Health
    public int currentHealth;                       // Player's current health

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Handle events after a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetReferences();

        sceneType = FindObjectOfType<SceneType>();

        if(sceneType.type == SceneType.SceneTypes.S_TwoD)
        {
            // rb.mass = 200f;                                                             // Set Player Mass
            rb.gravityScale = 1f;                                                      // Set Player Gravity Scale
            facingRight = false;                                                        // Set Player Face Direction
            /*
            Vector3 scale = transform.localScale;                                       // Set Player Scale
            scale.x = -2f;
            scale.y = 2f;
            transform.localScale = scale;
            */
            wSpeed = 6f;                                                                // Set Player Walk Speed
            rSpeed = 9f;                                                                // Set Player Run Speed
            transform.position = new Vector3(8.24f, 0.7f, transform.position.z);        // Set Player Position

        }
        else if(sceneType.type == SceneType.SceneTypes.S_TwoPointFiveD)
        {
            //rb.mass = 1f;                                                               // Set Player Mass
            // rb.gravityScale = 0f;                                                       // Set Player Gravity Scale
            facingRight = true;                                                         // Set Player Face Direction
            /*
            Vector3 scale = transform.localScale;                                       // Set Player Scale
            scale.x = 1f;
            scale.y = 1f;
            transform.localScale = scale;
            */
            wSpeed = 3f;                                                                // Set Player Walk Speed
            rSpeed = 6f;                                                                // Set Player Run Speed
            transform.position = new Vector3(-0.03f, -2.68f, transform.position.z);     // Set Player Position
        }
    }

    // Link Cached References
    void SetReferences()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
        sR = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        facingRight = true;
    }

    void FixedUpdate()
    {
        // Moving the player
        HandleMovement();

        // Flip the player sprite depending on where it is facing
        Flip();

        // Player attacks enemies
        HandleAttacks();

        // Reset parameters
        ResetValues();
    }

    private void HandleMovement()
    {
        float horizontal = playerInput.horizontalInput;
        float vertical = playerInput.verticalInput;
        bool run = playerInput.runInput;

        movement.x = horizontal;
        if (sceneType.type == SceneType.SceneTypes.S_TwoD)
        {
            movement.y = 0f;
        }
        else
        {
            movement.y = vertical;
        }

        movement.Normalize();

        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
        {
            rb.velocity = new Vector2(movement.x * movementSpeed, movement.y * movementSpeed);
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        if (run == true)
        {
            animator.SetBool("Run", true);
            movementSpeed = rSpeed;
        }

        if(run == false)
        {
            animator.SetBool("Run", false);
            movementSpeed = wSpeed;
        }
    }

    private void HandleAttacks()
    {
        if (playerInput.attack1Pressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1"))
        {
            animator.SetBool("Attack1", true);
            rb.velocity = Vector2.zero;
        }

        if (playerInput.attack2Pressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
        {
            animator.SetBool("Attack2", true);
            rb.velocity = Vector2.zero;
        }

        if (playerInput.attack1Released && this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1"))
        {
            animator.SetBool("Attack1", false);
        }

        if (playerInput.attack2Released && this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
        {
            animator.SetBool("Attack2", false);
        }
    }

    private void Flip()
    {
        float horizontal = movement.x;
        if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            // Multiply the scale of the player object to flip the sprite
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation

        healthBar.SetHealth(currentHealth);
    }

    private void ResetValues()
    {
        
    }
}
