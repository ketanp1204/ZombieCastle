﻿using System;
using System.Collections;
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

    // Private References
    private UIReferences uiReferences;                                              // Reference - Current UIReferences script in the scene

    private PlayerInput playerInput;                                                // Reference - PlayerInput class

    private PlayerCombat playerCombat;                                              // Reference - PlayerCombat class

    private Rigidbody2D rb;                                                         // Reference - Rigidbody component

    private Animator animator;                                                      // Reference - Animator component

    private HealthBar healthBar;                                                    // Reference - Health bar 

    private Animator torchAnimator;                                                 // Reference - Torch object's animator

    private Transform magicParticlesSpawnLocation;                                  // Reference - Sword magic attack particles spawn location

    private Transform fireballSpawnLocations;                                       // Reference - Fireball attack particles spawn location container


    // Public References
    public Transform pathfindingTarget;                                             // Reference - Pathfinding target
    public GameObject selectionArea;                                                // Reference - Player selection area - Used to prevent selection during game instructions display
    public GameObject torchGO;                                                      // Reference - Torch for room3
    public Transform combatCameraPosition;                                          // Reference - Transform that the player camera should follow when in combat


    // Public Variables
    [HideInInspector]
    public bool movePlayer;                                                         // Bool - Player can move

    [HideInInspector]
    public bool canAttack;                                                          // Bool - Player can attack

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

    private bool isInRoom3 = false;                                                 // Bool - If player is in room3 then this is used to flip the torchlight direction while moving


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
        HandleSceneChanges();
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
        torchAnimator = transform.Find("Torch").GetComponent<Animator>();
        magicParticlesSpawnLocation = transform.Find("MagicParticlesSpawnLocation");
        fireballSpawnLocations = transform.Find("FireballSpawnLocations");
    }

    // Default Values
    public void Initialize()
    {
        movePlayer = true;
        canAttack = true;
        movementSpeed = walkSpeed;
        PlayerStats.playerState = PlayerStats.PlayerState.Idle;

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
            healthBar.ShowHealthBarAfterDelay(0f);
            healthBar.HideHealthBarAfterDelay(3f);
        }
        else
        {
            currentHealth = PlayerStats.currentHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
            healthBar.ShowHealthBarAfterDelay(0f);
            healthBar.HideHealthBarAfterDelay(3f);
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
        
        animator.SetBool("IsDead", false);          // Not dead
        animator.SetBool("AxeDrawn", false);        // Axe unequipped
        animator.SetBool("KnifeDrawn", false);      // Knife unequipped
        animator.SetBool("SwordDrawn", false);      // Sword unequipped
        animator.SetBool("HoldingTorch", false);    // Not holding torch
    }

    private void HandleSceneChanges()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Room1")
        {
            GameData.sceneName = LevelManager.SceneNames.Room1;

            torchGO.SetActive(false);
            animator.SetBool("HoldingTorch", false);
            isInRoom3 = false;

            if (GameData.r1_combatCompleted)
            {
                InventoryManager.instance.EnableInteractionOfAllSlots();
            }
            else
            {
                InventoryManager.instance.DisableAllWeaponInteractions();
            }
        }
        else if (scene.name == "Room2")
        {
            GameData.sceneName = LevelManager.SceneNames.Room2;

            torchGO.SetActive(false);
            animator.SetBool("HoldingTorch", false);
            isInRoom3 = false;
        }
        else if (scene.name == "Room3")
        {
            GameData.sceneName = LevelManager.SceneNames.Room3;

            torchGO.SetActive(true);
            animator.SetBool("HoldingTorch", true);
            isInRoom3 = true;

            // Prevent weapon interactions
            if (InventoryManager.instance)
            {
                InventoryManager.instance.DisableAllWeaponInteractions();
            }
        }    
        else if (scene.name == "Room5")
        {
            GameData.sceneName = LevelManager.SceneNames.Room5;

            torchGO.SetActive(false);
            animator.SetBool("HoldingTorch", false);
            isInRoom3 = false;

            if (GameData.r5_zombie3CombatCompleted)
            {
                // Set player position to middle floor 
                transform.position = new Vector3(18.5f, -2.61f, 0f);
            }
            else
            {
                // Set player position to room 5 door
                transform.position = new Vector3(-22.12f, 7.58f, 0f);
            }
        }

        GameData.loadingCheckpointFromGameOver = false;
        SetPlayerFaceDirection(scene);
    }

    private void SetPlayerFaceDirection(Scene scene)
    {
        if (scene.name == "Room1" || scene.name == "Room2")
        {
            // Facing left
            facingRight = false;
            animator.SetFloat("FaceDir", -1f);          
        }
        else
        {
            // Facing right
            facingRight = true;
            animator.SetFloat("FaceDir", 1f);

            if (isInRoom3)
            {
                torchAnimator.SetFloat("FaceDir", 1f);
            }
        }
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
            }
        }
    }

    private void Update()
    {
        if (movePlayer)
        {
            // Handle attack input
            HandleAttacks();
        }
    }

    private void HandleMovement()
    {
        float horizontal;
        horizontal = playerInput.horizontalInput;

        movement.x = horizontal;
        movement.y = 0f;

        movement.Normalize();
        rb.velocity = new Vector2(movement.x * movementSpeed * Time.deltaTime, rb.velocity.y);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        if (isInRoom3)
        {
            torchAnimator.SetFloat("Magnitude", movement.magnitude);
        }
    }

    private void FlipPlayerDirection()
    {
        if (PlayerStats.playerState != PlayerStats.PlayerState.Combat)
        {
            float horizontal = movement.x;
            if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
            {
                facingRight = !facingRight;

                if (facingRight)
                {
                    // Set animation parameter
                    animator.SetFloat("FaceDir", 1f);

                    // Set sword particle spawn locations direction
                    if (magicParticlesSpawnLocation.transform.localPosition.x < 0)
                    {
                        magicParticlesSpawnLocation.eulerAngles = new Vector3(0f, 0f, 0f);
                        magicParticlesSpawnLocation.localPosition = new Vector3(-1f * magicParticlesSpawnLocation.localPosition.x, magicParticlesSpawnLocation.localPosition.y, magicParticlesSpawnLocation.localPosition.z);
                    }

                    if (fireballSpawnLocations.transform.localPosition.x < 0)
                    {
                        fireballSpawnLocations.eulerAngles = new Vector3(0f, 0f, 0f);
                        fireballSpawnLocations.localPosition = new Vector3(-1f * fireballSpawnLocations.localPosition.x, fireballSpawnLocations.localPosition.y, fireballSpawnLocations.localPosition.z);
                    }

                    if (isInRoom3)
                    {
                        // Set torch animator parameter
                        torchAnimator.SetFloat("FaceDir", 1f);
                    }
                }
                else
                {
                    // Set animation parameter
                    animator.SetFloat("FaceDir", -1f);

                    // Set sword particle spawn locations direction
                    if (magicParticlesSpawnLocation.transform.localPosition.x > 0)
                    {
                        magicParticlesSpawnLocation.eulerAngles = new Vector3(0f, 180f, 0f);
                        magicParticlesSpawnLocation.localPosition = new Vector3(-1f * magicParticlesSpawnLocation.localPosition.x, magicParticlesSpawnLocation.localPosition.y, magicParticlesSpawnLocation.localPosition.z);
                    }

                    if (fireballSpawnLocations.transform.localPosition.x > 0)
                    {
                        fireballSpawnLocations.eulerAngles = new Vector3(0f, 180f, 0f);
                        fireballSpawnLocations.localPosition = new Vector3(-1f * fireballSpawnLocations.localPosition.x, fireballSpawnLocations.localPosition.y, fireballSpawnLocations.localPosition.z);
                    }

                    if (isInRoom3)
                    {
                        // Set torch animator parameter
                        torchAnimator.SetFloat("FaceDir", -1f);
                    }
                }
            }
        }
    }

    private void HandleAttacks()
    {
        if (canAttack)
        {
            if (weaponEquippedDict[PlayerCombat.WeaponTypes.Axe])
            {
                if (Input.GetMouseButtonDown(0) && !takingDamage && !PlayerStats.IsDead)
                {
                    playerCombat.InvokeAxeAttack();
                    rb.velocity = Vector2.zero;
                }
            }
            else if (weaponEquippedDict[PlayerCombat.WeaponTypes.Knife])
            {
               
                if (Input.GetMouseButtonDown(0) && !takingDamage && !PlayerStats.IsDead)
                {
                    playerCombat.InvokeKnifeAttack();
                    rb.velocity = Vector2.zero;
                }
            }
            else if (weaponEquippedDict[PlayerCombat.WeaponTypes.Sword])
            {
                if (Input.GetMouseButtonDown(0) && !takingDamage && !PlayerStats.IsDead)
                {
                    // TODO: different behavior for magic potion + sword, fire elem + sword and sword without any powers
                    if (playerCombat.swordAttackType == PlayerCombat.SwordAttackTypes.Normal)
                    {
                        playerCombat.InvokeSwordNormalAttack();
                        rb.velocity = Vector2.zero;
                    }
                    else if (playerCombat.swordAttackType == PlayerCombat.SwordAttackTypes.Fire)
                    {
                        playerCombat.InvokeSwordFireAttack();
                        rb.velocity = Vector2.zero;
                    }
                    else if (playerCombat.swordAttackType == PlayerCombat.SwordAttackTypes.Magic)
                    {
                        playerCombat.InvokeSwordMagicAttack();
                        rb.velocity = Vector2.zero;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space) && !PlayerStats.IsDead)
                {
                    playerCombat.InvokeSwordBlock();
                    rb.velocity = Vector2.zero;
                }
            }
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

    public void SetClimbingLadderDown()
    {
        isClimbingLadder = true;
        if (isInRoom3)
            torchAnimator.SetBool("LadderClimbDown", true);
    }

    public void UnsetClimbingLadderDown()
    {
        isClimbingLadder = false;
        if (isInRoom3)
            torchAnimator.SetBool("LadderClimbDown", false);
    }

    public void SetClimbingLadderUp()
    {
        isClimbingLadder = true;
        if (isInRoom3)
            torchAnimator.SetBool("LadderClimbUp", true);
    }

    public void UnsetClimbingLadderUp()
    {
        isClimbingLadder = false;
        if(isInRoom3)
            torchAnimator.SetBool("LadderClimbUp", false);
    }

    public void UpdateCombatCameraPosition(Transform enemy)
    {
        // Enemy is to the right of the player
        if (enemy.position.x > transform.position.x)
        {
            combatCameraPosition.localPosition = new Vector3(1f * combatCameraPosition.localPosition.x, combatCameraPosition.localPosition.y, combatCameraPosition.localPosition.z);
        }
        else
        {
            // Enemy is to the left of the player
            combatCameraPosition.localPosition = new Vector3(-1f * combatCameraPosition.localPosition.x, combatCameraPosition.localPosition.y, combatCameraPosition.localPosition.z);
        }
    }

    public void PlayFootStepSound()
    {
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerFootStep);
    }

    public void PlayLadderFootStepSound()
    {
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.LadderFootStep);
    }

    public void PlayLowHealthBreathingSound()
    {
        if (PlayerStats.currentHealth < 30)
        {
            AudioManager.PlayPersistentSingleSoundIfNotAlreadyPlaying(AudioManager.Sound.PlayerLowHealthBreathing);
        }
    }

    public static void SetIdleState()
    {
        PlayerStats.playerState = PlayerStats.PlayerState.Idle;
        instance.healthBar.HideHealthBarAfterDelay(2.5f);
    }

    public static void SetCombatState()
    {
        if (PlayerStats.playerState != PlayerStats.PlayerState.Combat)
        {
            PlayerStats.playerState = PlayerStats.PlayerState.Combat;
            instance.healthBar.ShowHealthBarAfterDelay(1.5f);
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

    public void MakePlayerFaceLeft()
    {
        facingRight = false;
        animator.SetFloat("FaceDir", -1f);
    }

    // Stop input for movement
    public static void StopMovement()
    {
        if (instance != null)
        {
            instance.movePlayer = false;
            instance.rb.velocity = Vector2.zero;

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

    public static void DisableAttackInput()
    {
        if (instance != null)
        {
            instance.canAttack = false;
        }
    }

    // Enable input for attack after delay
    public static void EnableAttackInputAfterDelay()
    {
        if (instance != null)
        {
            new Task(instance.EnableAttackAfterDelay(0.3f));
        }
    }

    private IEnumerator EnableAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        canAttack = true;
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
