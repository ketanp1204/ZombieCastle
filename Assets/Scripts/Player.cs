using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Singleton
    public static Player instance;

    // Cached References
    private Rigidbody2D rb;
    private Animator animator;
    public HealthBar healthBar;

    // Variables
    [SerializeField]
    public float movementSpeed;
    private bool facingRight;
    private bool run;
    private Vector2 movement;
    public int maxHealth = 100;
    public int currentHealth;

    // Variables : Player Attack1
    private bool attack1;
    private bool attack2;

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

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        facingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        run = Input.GetKey(KeyCode.LeftShift);

        movement.x = horizontal;
        movement.y = vertical;

        // Moving the player
        HandleMovement(movement.normalized);

        // Flip the player sprite depending on where it is facing
        Flip(movement.x);

        // Player attacks enemies
        HandleAttacks();

        // Reset parameters
        ResetValues();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            attack1 = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            attack2 = true;
        }
    }

    private void HandleMovement(Vector2 movement)
    {
        if(SceneManager.GetActiveScene().name != "Room1")       // Temporary. TODO: remove
        {
            if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
            {
                rb.velocity = new Vector2(movement.x * movementSpeed, movement.y * movementSpeed);
            }

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Magnitude", movement.magnitude);
            if (run)
            {
                animator.SetBool("Run", true);
                movementSpeed = 6f;
            }
            else
            {
                animator.SetBool("Run", false);
                movementSpeed = 3f;
            }
        }
        

    }

    private void HandleAttacks()
    {
        if(attack1 && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1"))
        {
            animator.SetTrigger("Attack1");
            rb.velocity = Vector2.zero;
        }

        if (attack2 && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2"))
        {
            animator.SetTrigger("Attack2");
            rb.velocity = Vector2.zero;
        }
    }

    private void Flip(float horizontal)
    {
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

        healthBar.SetHealth(currentHealth);
    }

    private void ResetValues()
    {
        attack1 = false;
        attack2 = false;
    }
}
