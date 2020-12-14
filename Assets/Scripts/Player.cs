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
    public HealthBar healthBar;                     // Reference To The Player's Health Bar 
    private SceneType sceneTypeReference;           // Reference To The Current Type Of Scene: 2D or 2.5D
    private SpriteRenderer sR;                      // Reference To The Player GameObject's SpriteRenderer

    // Variables : Player
    public float movementSpeed;                     // Player's Current Movement Speed;
    public float wSpeed;                            // Player's Walking Speed
    public float rSpeed;                            // Player's Running Speed
    private bool facingRight;                       // Player's Direction Of Facing
    private Vector2 movement;                       // Player's Movement Vector
    public int maxHealth = 100;                     // Player's Maximum Health
    public int currentHealth;                       // Player's current health
    private int sceneType;                          // Type of scene: 1 for 2D, 2 for 2.5D

    /*
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
    */

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Handle events after a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    // Link Cached References
    void SetReferences()
    {
        sceneTypeReference = FindObjectOfType<SceneType>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

        SetReferences();

        if (sceneTypeReference.type == SceneType.SceneTypes.S_TwoD)
        {
            sceneType = 1;
        }
        else if (sceneTypeReference.type == SceneType.SceneTypes.S_TwoPointFiveD)
        {
            sceneType = 2;
        }

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
            
        facingRight = true;
    }

    void FixedUpdate()
    {
        // Moving the player
        if(sceneType == 1)
        {
            HandleMovementTwoD();
        }
        else if(sceneType == 2)
        {
            HandleMovementTwoPointFiveD();
        }
        

        // Flip the player sprite depending on where it is facing
        Flip();

        // Player attacks enemies
        HandleAttacks();

        // Reset parameters
        ResetValues();
    }

    private void HandleMovementTwoD()
    {
        float horizontal = playerInput.horizontalInput;
        bool run = playerInput.runInput;

        movement.x = horizontal;

        movement.Normalize();

        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
        {
            rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Magnitude", movement.magnitude);

        if (run == true)
        {
            animator.SetBool("Run", true);
            movementSpeed = rSpeed;
        }

        if (run == false)
        {
            animator.SetBool("Run", false);
            movementSpeed = wSpeed;
        }
    }

    private void HandleMovementTwoPointFiveD()
    {
        float horizontal = playerInput.horizontalInput;
        float vertical = playerInput.verticalInput;
        bool run = playerInput.runInput;

        movement.x = horizontal;
        movement.y = vertical;
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

    public void AddVerticalVelocity(float vertical)
    {
        rb.velocity = new Vector2(rb.velocity.x, vertical);
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
        PlayerStats.currentHealth = currentHealth;

        // Play hurt animation
        animator.SetTrigger("Hurt");

        healthBar.SetHealth(currentHealth);
    }

    private void ResetValues()
    {
        
    }
}
