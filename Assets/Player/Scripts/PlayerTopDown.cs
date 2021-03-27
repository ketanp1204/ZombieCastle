using System;
using System.Collections;
using System.Collections.Generic;
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


    // Private References
    private UIReferences uiReferences;                                              // Reference - Current UIReferences script in the scene

    private PlayerInput playerInput;                                                // Reference - PlayerInput class

    private PlayerCombat playerCombat;                                              // Reference - PlayerCombat class

    private Rigidbody2D rb;                                                         // Reference - Rigidbody component

    private Animator animator;                                                      // Reference - Animator component

    private HealthBar healthBar;                                                    // Reference - Health bar 


    // Public References
    public GameObject selectionArea;                                                // Reference - Player selection area - Used to prevent selection during game instructions display


    // Public Variables
    [HideInInspector]
    public bool movePlayer;                                                         // Bool - Player can move

    [HideInInspector]
    public bool IsDead = false;                                                     // Bool - Player dead

    [HideInInspector]
    public float movementSpeed;                                                     // Float - Current movement speed;

    [HideInInspector]
    public int currentHealth;                                                       // Int - Current health

    [HideInInspector]
    public bool takingDamage;                                                       // Bool - Currently taking damage

    public Dictionary<PlayerCombat.WeaponTypes, bool> weaponEquippedDict;           // Dictionary - Weapons and their equipped status

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
        ResetAnimatorParameters();

        if (PlayerStats.isFirstScene)
        {
            StopMovement();
        }
        else
        {
            HandleSceneChanges();
        }
        
    }

    // Store References
    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        healthBar = uiReferences.playerHealthBar;
        
        playerInput = GetComponent<PlayerInput>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Default Values
    public void Initialize()
    {
        // Movement
        facingRight = true;
        movePlayer = true;
        movementSpeed = walkSpeed;
        animator.SetFloat("FaceDir", 1f);

        // Combat
        takingDamage = false;
        weaponEquippedDict = new Dictionary<PlayerCombat.WeaponTypes, bool>();
        weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = false;
        weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = false;
        weaponEquippedDict[PlayerCombat.WeaponTypes.Sword] = false;

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

    private void ResetAnimatorParameters()
    {
        animator.SetFloat("Horizontal", 0f);        // No keyboard input
        animator.SetFloat("Vertical", 0f);          // No keyboard input
        animator.SetFloat("Magnitude", 0f);         // No keyboard input
        animator.SetFloat("FaceDir", 1f);           // Facing right
        animator.SetBool("IsDead", false);          // Not dead
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
        else if (GameData.sceneName == LevelManager.SceneNames.Room5)
        {
            // TODO: remove later 
            transform.position = new Vector3(5.54f, -3.97f, 0f);
        }
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
        float horizontal = playerInput.horizontalInput;
        float vertical = playerInput.verticalInput;

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
    }

    public void PlayFootStepSound()
    {
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerFootStep);
    }

    public void PlayLowHealthBreathingSound()
    {
        // TODO: add low health breathing sound
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
        if (weaponEquippedDict[PlayerCombat.WeaponTypes.Axe])
        {
            /*
            if (playerInput.leftMousePressed && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt") && !PlayerStats.IsDead)
            {
                playerCombat.InvokeAxeAttack();
                animator.SetTrigger("AxeAttack");
                rb.velocity = Vector2.zero;
            }
            */
            if (playerInput.leftMousePressed && !playerCombat.isAttacking_Axe && !takingDamage && !PlayerStats.IsDead)
            {
                playerCombat.InvokeAxeAttack();
                animator.SetTrigger("AxeAttack");
                rb.velocity = Vector2.zero;
            }
        }

        if (weaponEquippedDict[PlayerCombat.WeaponTypes.Knife])
        {
            if (playerInput.leftMousePressed && !playerCombat.isAttacking_Knife && !takingDamage && !PlayerStats.IsDead)
            {
                playerCombat.InvokeKnifeAttack();
                animator.SetTrigger("KnifeAttack");
                rb.velocity = Vector2.zero;
            }
        }

        if (weaponEquippedDict[PlayerCombat.WeaponTypes.Sword])
        {
            if (playerInput.leftMousePressed && !takingDamage && !PlayerStats.IsDead)
            {
                // TODO: different behavior for magic potion + sword, fire elem + sword and sword without any powers
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

    /// <summary>
    /// Weapons equip/unequip
    /// </summary>
    /// <returns></returns>


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

    // Returns true if sword is equipped
    public static bool SwordDrawn()
    {
        if (instance != null)
        {
            return instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Sword];
        }
        else
            return false;
    }

    // Sets the AxeDrawn animation parameter to true
    public static void EquipAxe()
    {
        instance.StartCoroutine(instance.EquipAxeAfterDelay());
    }

    private IEnumerator EquipAxeAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = true;
            instance.animator.SetBool("AxeDrawn", true);

            // Set other weapon bools to false
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = false;
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Sword] = false;
            instance.animator.SetBool("KnifeDrawn", false);
            instance.animator.SetBool("SwordDrawn", false);
        }
    }

    // Sets the AxeDrawn animation parameter to false
    public static void UnequipAxe()
    {
        instance.StartCoroutine(instance.UnequipAxeAfterDelay());
    }

    private IEnumerator UnequipAxeAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = false;
            instance.animator.SetBool("AxeDrawn", false);
        }
    }

    // Sets the KnifeDrawn animation parameter to true
    public static void EquipKnife()
    {
        instance.StartCoroutine(instance.EquipKnifeAfterDelay());
    }

    private IEnumerator EquipKnifeAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = true;
            instance.animator.SetBool("KnifeDrawn", true);

            // Set other weapon equipped bools to false
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = false;
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Sword] = false;
            instance.animator.SetBool("AxeDrawn", false);
            instance.animator.SetBool("SwordDrawn", false);
        }
    }

    // Sets the KnifeDrawn animation parameter to false
    public static void UnequipKnife()
    {
        instance.StartCoroutine(instance.UnequipKnifeAfterDelay());
    }

    private IEnumerator UnequipKnifeAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = false;
            instance.animator.SetBool("KnifeDrawn", false);
        }
    }

    // Sets the SwordDrawn animation parameter to true
    public static void EquipSword()
    {
        instance.StartCoroutine(instance.EquipSwordAfterDelay());
    }

    private IEnumerator EquipSwordAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Sword] = true;
            instance.animator.SetBool("SwordDrawn", true);

            // Set other weapon bools to false
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Knife] = false;
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Axe] = false;
            instance.animator.SetBool("KnifeDrawn", false);
            instance.animator.SetBool("AxeDrawn", false);
        }
    }

    // Sets the SwordDrawn animation parameter to false
    public static void UnequipSword()
    {
        instance.StartCoroutine(instance.UnequipSwordAfterDelay());
    }

    private IEnumerator UnequipSwordAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (instance != null)
        {
            instance.weaponEquippedDict[PlayerCombat.WeaponTypes.Sword] = false;
            instance.animator.SetBool("SwordDrawn", false);
        }
    }
}
