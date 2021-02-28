using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerCombat))]

public class PlayerTopDown : MonoBehaviour
{
    // Singleton
    public static PlayerTopDown instance;

    // Private Cached References
    private PlayerInput playerInput;                // Reference to the PlayerInput Class
    private PlayerCombat playerCombat;              // Reference to the PlayerCombat Class
    private Rigidbody2D rb;                         // Reference to the Player's Rigidbody Component
    private Animator animator;                      // Reference to the Player's Animator Component
    private UnityEngine.Object playerReference;     // Reference to The Player Gameobject references used to respawn

    // Public Cached References
    public GameObject rightSelectionArea;           // Reference to the right side selection area collider
    public GameObject leftSelectionArea;            // Reference to the left side selection area collider
    public HealthBar healthBar;                     // Reference to the Player's Health Bar 
    public InventoryObject inventory;               // Reference to the Player's inventory system object

    // Variables : Player
    [HideInInspector]
    public bool movePlayer;                         // Bool to store whether Player can move
    [HideInInspector]
    public bool axeDrawn;                           // Bool to store whether axe is drawn
    [HideInInspector]
    public bool knifeDrawn;                         // Bool to store whether knife is drawn
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

    // Death Event
    public event Action<PlayerTopDown> OnDeath;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        SetReferences();
        InitializeValues();

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
    }

    private void InitializeValues()
    {
        facingRight = true;
        movePlayer = true;
        axeDrawn = false;
        knifeDrawn = false;
        movementSpeed = walkSpeed;
        animator.SetFloat("FaceDir", 1f);

        rightSelectionArea.SetActive(true);
        leftSelectionArea.SetActive(false);
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
        float horizontal = playerInput.horizontalInput;
        float vertical = playerInput.verticalInput;
        bool walkFast = playerInput.walkFastInput;

        movement.x = horizontal;
        movement.y = vertical;
        movement.Normalize();

        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            rb.MovePosition((Vector2)transform.position + (movement * movementSpeed * Time.deltaTime));
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        /*
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
        */
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

                // Use right side selection area
                rightSelectionArea.SetActive(true);
                leftSelectionArea.SetActive(false);
            }
            else
            {
                // Set animation parameter
                animator.SetFloat("FaceDir", -1f);

                // Use left side selection area
                leftSelectionArea.SetActive(true);
                rightSelectionArea.SetActive(false);
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
        if (axeDrawn)
        {
            if (playerInput.leftMousePressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                playerCombat.InvokeAxeAttack();
                animator.SetTrigger("AxeAttack");
                rb.velocity = Vector2.zero;
            }
        }

        if (knifeDrawn)
        {
            if (playerInput.leftMousePressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                playerCombat.InvokeKnifeAttack();
                animator.SetTrigger("KnifeAttack");
                rb.velocity = Vector2.zero;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsDead)
        {
            // Reduce health
            currentHealth -= damage;
            PlayerStats.currentHealth = currentHealth;

            // Update Health Bar
            healthBar.SetHealth(currentHealth);

            // Play hurt animation
            animator.SetTrigger("Hurt");

            // Die if health is less than 0
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        IsDead = true;
        movePlayer = false;

        // Play die animation
        animator.SetBool("IsDead", true);

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        // Respawn player
        // Respawn();

        StartCoroutine(DestroyGameObjectAfterDelay(gameObject));
    }

    void Respawn()
    {
        GameObject playerClone = (GameObject)Instantiate(playerReference);
        playerClone.transform.position = transform.position;
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
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
        instance.animator.SetFloat("Horizontal", 0f);
        instance.animator.SetFloat("Vertical", 0f);
        instance.animator.SetFloat("Magnitude", 0f);
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
            rb.velocity = Vector2.zero;
            animator.SetFloat("Magnitude", 0f);
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
