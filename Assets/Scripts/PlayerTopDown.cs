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
    private PlayerInput playerInput;                // Reference To The PlayerInput Class
    private PlayerCombat playerCombat;              // Reference To The PlayerCombat Class
    private Rigidbody2D rb;                         // Reference To The Player's Rigidbody Component
    private Animator animator;                      // Reference To The Player's Animator Component
    private HealthBar healthBar;                    // Reference To The Player's Health Bar 
    private SceneType sceneTypeReference;           // Reference To The Current Type Of Scene: 2D or 2.5D
    private UnityEngine.Object playerReference;     // Reference to The Player Gameobject references used to respawn

    // Public Cached References
    public GameObject rightSelectionArea;           // Reference to the right side selection area collider
    public GameObject leftSelectionArea;            // Reference to the left side selection area collider

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
    public float wSpeed;                            // Player's Walking Speed
    public float rSpeed;                            // Player's Running Speed
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

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();

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

        InitializeValues();
    }

    private void InitializeValues()
    {
        facingRight = true;
        movePlayer = true;
        weaponDrawn = false;
        movementSpeed = wSpeed;
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

        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
        {
            rb.MovePosition((Vector2)transform.position + (movement * movementSpeed * Time.deltaTime));
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        if (walkFast == true)
        {
            animator.SetBool("WalkFast", true);
            movementSpeed = rSpeed;
        }

        if (walkFast == false)
        {
            animator.SetBool("WalkFast", false);
            movementSpeed = wSpeed;
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
        if (weaponDrawn)
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
                Debug.Log("attack1_released");
                animator.SetBool("Attack1", false);
            }

            if (playerInput.attack2Released && this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
            {
                animator.SetBool("Attack2", false);
            }
            */
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
            rb.velocity = Vector2.zero;
            animator.SetFloat("Magnitude", 0f);
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
