using System;
using System.Collections.Generic;
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
    private UIReferences uiReferences;                                              // Reference - Current UIReferences script in the scene

    private PlayerInput playerInput;                                                // Reference - PlayerInput class

    private PlayerCombat playerCombat;                                              // Reference - PlayerCombat class

    private Rigidbody2D rb;                                                         // Reference - Rigidbody component

    private Animator animator;                                                      // Reference - Animator component

    private HealthBar healthBar;                                                    // Reference - Health bar 

    private InventoryObject inventory;                                              // Reference - Inventory ScriptableObject

    // Public Cached References
    public Transform pathfindingTarget;                                             // Reference - Pathfinding target


    // Public Variables
    [HideInInspector]
    public bool movePlayer;                                                         // Bool - Player can move

    [HideInInspector]
    public bool isClimbingLadder = false;                                           // Bool - Player climbing a ladder

    [HideInInspector]
    public float movementSpeed;                                                     // Float - Current movement speed

    [HideInInspector]
    public int currentHealth;                                                       // Int - Current health

    [HideInInspector]
    public bool takingDamage;                                                       // Bool - Currently taking damage

    public Dictionary<PlayerCombat.WeaponTypes, bool> weaponEquippedDict;           // Dictionary - Weapons and their equipped status

    public float walkSpeed;                                                         // Float - Walk speed

    public int maxHealth = 100;                                                     // Int - Maximum health

    // Private Variables
    private bool facingRight;                                                       // Player's direction of facing

    private Vector2 movement;                                                       // Vector2 - Movement vector


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        SetReferences();
        Initialization();
    }

    void OnEnable()
    {
        SetReferences();
        ResetAnimatorParameters();
    }

    // Store References
    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        healthBar = uiReferences.playerHealthBar;
        inventory = uiReferences.playerInventory;

        playerInput = GetComponent<PlayerInput>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Default Values
    private void Initialization()
    {
        facingRight = false;
        movePlayer = true;
        movementSpeed = walkSpeed;
        animator.SetFloat("FaceDir", -1f);

        // Combat
        takingDamage = false;
        weaponEquippedDict = new Dictionary<PlayerCombat.WeaponTypes, bool>();
        weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = false;
        weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = false;

        // Health
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

        // Ignore player collisions with dead zombies
        Physics2D.IgnoreLayerCollision(gameObject.layer, 13);

        // Ignore player collision with maze collider
        Physics2D.IgnoreLayerCollision(gameObject.layer, 14);
    }

    private void ResetAnimatorParameters()
    {
        animator.SetFloat("Horizontal", 0f);        // No keyboard input
        animator.SetFloat("Vertical", 0f);          // No keyboard input
        animator.SetFloat("Magnitude", 0f);         // No keyboard input
        animator.SetFloat("FaceDir", -1f);          // Facing left
        animator.SetBool("IsDead", false);          // Not dead
        animator.SetBool("AxeDrawn", false);        // Axe unequipped
        animator.SetBool("KnifeDrawn", false);      // Knife unequipped
    }

    void FixedUpdate()
    {
        // Player movement
        if (movePlayer)
        {
            if (!this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !takingDamage)
            {
                HandleMovement();

                // Flip the player direction depending on where he is facing
                FlipPlayerDirection();

                // Player attacks enemies
                HandleAttacks();
            }
        }
    }

    private void HandleMovement()
    {
        float horizontal, vertical;
        horizontal = playerInput.horizontalInput;

        movement.x = horizontal;
        movement.y = 0f;

        if (isClimbingLadder)
        {
            vertical = playerInput.verticalInput;
            movement.y = vertical;
        }

        movement.Normalize();
        rb.velocity = new Vector2(movement.x * movementSpeed * Time.deltaTime, rb.velocity.y);
        
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

    private void HandleAttacks()
    {
        if(weaponEquippedDict[PlayerCombat.WeaponTypes.Axe])
        {
            if (playerInput.leftMousePressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt") && !PlayerStats.IsDead)
            {
                playerCombat.InvokeAxeAttack();
                animator.SetTrigger("AxeAttack");
                rb.velocity = Vector2.zero;
            }
        }

        if (weaponEquippedDict[PlayerCombat.WeaponTypes.Knife])
        {
            if (playerInput.leftMousePressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt") && !PlayerStats.IsDead)
            {
                playerCombat.InvokeKnifeAttack();
                animator.SetTrigger("KnifeAttack");
                rb.velocity = Vector2.zero;
            }
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

    // Returns the inventory scriptable object attached to this player
    public static InventoryObject GetInventory()
    {
        return instance.inventory;
    }

    // Returns true if axe is equipped
    public static bool AxeDrawn()
    {
        if (instance != null)
        {
            return instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe];
        }
        else
            return false;
    }

    // Returns true if knife is equipped
    public static bool KnifeDrawn()
    {
        if (instance != null)
        {
            return instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife];
        }
        else
            return false;
    }

    // Sets the AxeDrawn animation parameter to true
    public static void EquipAxe()
    {
        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = true;
            instance.animator.SetBool("AxeDrawn", true);

            // Set other weapon bools to false
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = false;
            instance.animator.SetBool("KnifeDrawn", false);
        }
    }

    // Sets the AxeDrawn animation parameter to false
    public static void UnequipAxe()
    {
        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = false;
            instance.animator.SetBool("AxeDrawn", false);
        }
    }

    // Sets the KnifeDrawn animation parameter to true
    public static void EquipKnife()
    {
        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = true;
            instance.animator.SetBool("KnifeDrawn", true);

            // Set other weapon equipped bools to false
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = false;
            instance.animator.SetBool("AxeDrawn", false);
        }
    }

    // Sets the KnifeDrawn animation parameter to false
    public static void UnequipKnife()
    {
        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = false;
            instance.animator.SetBool("KnifeDrawn", false);
        }
    }
}
