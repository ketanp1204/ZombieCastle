using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCombat : MonoBehaviour
{
    // Singleton
    public static PlayerCombat instance;

    public enum WeaponTypes
    {
        Knife,
        Axe,
        Sword
    }

    public enum SwordAttackTypes
    {
        Normal,
        Fire,
        Magic
    }
    
    [Header("Axe Attack")]
    public int axeDamage = 20;                                                          // Int - Damage amount inflicted on enemy
    public float axeAttackRepeatTime = 1f;                                              // Float - Delay between successive attacks
    public Animator axeHitboxAnimator_L;                                                // Reference - Left hitbox animator
    public Animator axeHitboxAnimator_R;                                                // Reference - Right hitbox animator

    [Header("Knife Attack")]
    public int knifeDamage = 10;                                                        // Int - Damage amount inflicted on enemy
    public float knifeAttackRepeatTime = 1f;                                            // Float - Delay between successive attacks
    public Animator knifeHitboxAnimator_L;                                              // Reference - Left hitbox animator
    public Animator knifeHitboxAnimator_R;                                              // Reference - Right hitbox animator

    [HideInInspector]
    public SwordAttackTypes swordAttackType = SwordAttackTypes.Normal;

    [Header("Sword Magic Attack")]
    public float swordMagicAttackRepeatTime = 0.8f;                                     // Float - Delay between successive attacks
    public Transform magicParticlesSpawnLocation;                                       // Transform - Position from which to spawn the magic spell particles
    public GameObject magicParticlePrefab;                                              // GameObject - Prefab of the player magic spell moving particle

    [Header("Sword Fire Attack")]
    public float swordFireAttackRepeatTime = 0.8f;                                      // Float - Delay between successive attacks
    public GameObject fireParticlesSpawnLocations;                                      // GameObject - Container for positions from which to spawn the fire particles
    public GameObject fireballPrefab;                                                   // GameObject - Prefab of the player fireball moving particle

    [Header("Sword Normal (using fire anim) Attack")]
    public int swordNormalDamage = 10;                                                  // Int - Damage amount inflicted on enemy
    public float swordNormalAttackRepeatTime = 1f;                                      // Float - Delay between successive attacks
    public Animator swordFireHitboxAnimator_L;                                          // Reference - Left hitbox animator
    public Animator swordFireHitboxAnimator_R;                                          // Reference - Right hitbox animator


    [Header("Common Attributes")]
    public LayerMask enemyLayers;                                                       // LayerMask - Enemies

    public float pushDistanceOnHit;                                                     // Float - Distance by which player is pushed back on taking damage

    [HideInInspector]
    public bool isAttacking_Axe = false;                                                // Bool - Axe attack status

    [HideInInspector]
    public bool isAttacking_Knife = false;                                              // Bool - Knife attack status

    [HideInInspector]
    public bool isAttacking_SwordNormal = false;                                        // Bool - Sword normal attack status

    [HideInInspector]
    public bool isBlocking_Sword = false;                                               // Bool - Block using sword status

    [HideInInspector]
    public bool isAttacking_SwordMagic = false;                                         // Bool - Sword magic attack status

    [HideInInspector]
    public bool isAttacking_SwordFire = false;                                          // Bool - Sword fire attack status

    [HideInInspector]
    public bool canAttack = false;                                                      // Bool - Player allowed to attack

    public event Action<PlayerCombat> OnDeath;                                          // Death Event

    // Private variables
    private Task attackTask = null;

    // Private Cached References
    private UIReferences uiReferences;                                                  // Reference - Current UIReferences script in the scene

    private HealthBar healthBar;                                                        // Reference - Player health bar

    private Animator animator;                                                          // Reference - Player animator

    // Public Cached References
    public Transform bloodParticlesStartPosition;                                       // Transform - Blood particles spawn position
    

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        uiReferences = FindObjectOfType<UIReferences>();
        healthBar = uiReferences.playerHealthBar;
        animator = GetComponent<Animator>();
    }

    public void StopAttack()
    {
        if (canAttack == true)
        {
            canAttack = false;

            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            ResetHitboxes();
            ResetAttackingBools();
        }
    }

    private void ResetHitboxes()
    {
        axeHitboxAnimator_L.enabled = false;
        axeHitboxAnimator_R.enabled = false;
        knifeHitboxAnimator_L.enabled = false;
        knifeHitboxAnimator_R.enabled = false;
        swordFireHitboxAnimator_L.enabled = false;
        swordFireHitboxAnimator_R.enabled = false;
    }

    private void ResetAttackingBools()
    {
        isAttacking_Axe = false;
        isAttacking_Knife = false;
        isAttacking_SwordNormal = false;
        isAttacking_SwordFire = false;
        isAttacking_SwordMagic = false;
    }

    public void InvokeAxeAttack()
    {
        canAttack = true;

        if (!isAttacking_Axe && !PlayerStats.IsDead)
        {
            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            attackTask = new Task(AttackAxe());
        }
    }

    private IEnumerator AttackAxe()
    {
        bool facingRight = Player.PlayerFacingRight();
        if (canAttack && !Player.instance.takingDamage)
        {
            if (PlayerStats.IsDead)
            {
                canAttack = false;
                isAttacking_Axe = false;
                yield break;
            }

            isAttacking_Axe = true;

            // Play player axe attack animation
            animator.SetTrigger("AxeAttack");

            // Animate hitbox
            if (facingRight)
            {
                axeHitboxAnimator_R.enabled = true;
                axeHitboxAnimator_R.SetTrigger("Attack");
            }
            else
            {
                axeHitboxAnimator_L.enabled = true;
                axeHitboxAnimator_L.SetTrigger("Attack");
            }

            // Wait for attack to end
            yield return new WaitForSeconds(axeAttackRepeatTime);
        }
        isAttacking_Axe = false;
    }

    public void PlayAxeAttackSound()
    {
        // Play axe attack sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerAxeAttack);
    }

    public void InvokeKnifeAttack()
    {
        canAttack = true;

        if (!isAttacking_Knife && !PlayerStats.IsDead)
        {
            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            attackTask = new Task(AttackKnife());
        }
    }

    private IEnumerator AttackKnife()
    {
        bool facingRight = Player.PlayerFacingRight();
        if (canAttack && !Player.instance.takingDamage)
        {
            if (PlayerStats.IsDead)
            {
                canAttack = false;
                isAttacking_Knife = false;
                yield break;
            }

            isAttacking_Knife = true;

            // Play player knife attack animation
            animator.SetTrigger("KnifeAttack");

            // Animate hitbox
            if (facingRight)
            {
                knifeHitboxAnimator_R.enabled = true;
                knifeHitboxAnimator_R.SetTrigger("Attack");
            }
            else
            {
                knifeHitboxAnimator_L.enabled = true;
                knifeHitboxAnimator_L.SetTrigger("Attack");
            }

            // Wait for attack to end
            yield return new WaitForSeconds(knifeAttackRepeatTime);
        }
        isAttacking_Knife = false;
    }

    public void PlayKnifeAttackSound()
    {
        // Play knife attack sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerKnifeAttack);
    }

    public void InvokeSwordNormalAttack()
    {
        canAttack = true;

        if (!isAttacking_SwordNormal && !PlayerStats.IsDead)
        {
            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            attackTask = new Task(AttackSwordNormal());
        }
    }

    private IEnumerator AttackSwordNormal()
    {
        bool facingRight = Player.PlayerFacingRight();
        if (canAttack && !Player.instance.takingDamage)
        {
            if (PlayerStats.IsDead)
            {
                canAttack = false;
                isAttacking_SwordNormal = false;
                yield break;
            }

            isAttacking_SwordNormal = true;

            // Play player sword fire attack animation
            animator.SetTrigger("SwordFireAttack");

            if (facingRight)
            {
                swordFireHitboxAnimator_R.enabled = true;
                swordFireHitboxAnimator_R.SetTrigger("Attack");
            }
            else
            {
                swordFireHitboxAnimator_L.enabled = true;
                swordFireHitboxAnimator_L.SetTrigger("Attack");
            }

            // Wait for attack to end
            yield return new WaitForSeconds(swordNormalAttackRepeatTime);
        }
        isAttacking_SwordNormal = false;
    }

    public void PlaySwordNormalAttackSound()
    {
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerSwordAttack);
    }

    public void InvokeSwordMagicAttack()
    {
        canAttack = true;

        if (!isAttacking_SwordMagic && !PlayerStats.IsDead)
        {
            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            attackTask = new Task(AttackSwordMagic());
        }
    }

    private IEnumerator AttackSwordMagic()
    {
        if (canAttack && !Player.instance.takingDamage)
        {
            if (PlayerStats.IsDead)
            {
                canAttack = false;
                isAttacking_SwordMagic = false;
                yield break;
            }

            isAttacking_SwordMagic = true;

            // Play player sword magic attack animation
            animator.SetTrigger("SwordMagicAttack");

            // Spawn magic particles
            new Task(SpawnMagicParticles());

            // Wait for attack to end
            yield return new WaitForSeconds(swordMagicAttackRepeatTime);
        }
        isAttacking_SwordMagic = false;
    }

    private IEnumerator SpawnMagicParticles()
    {
        yield return new WaitForSeconds(0.15f);

        // Play magic particles spawn sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.PlayerMagicSpellSpawn);

        // Wait for anim to reach magic particle spawn location
        yield return new WaitForSeconds(0.30f);

        // Spawn magic particles
        Instantiate(magicParticlePrefab, magicParticlesSpawnLocation.position, magicParticlesSpawnLocation.localRotation);

        yield return new WaitForSeconds(0.1f);

        Instantiate(magicParticlePrefab, magicParticlesSpawnLocation.position, magicParticlesSpawnLocation.localRotation);

        yield return new WaitForSeconds(0.3f);

        Instantiate(magicParticlePrefab, magicParticlesSpawnLocation.position, magicParticlesSpawnLocation.localRotation);
    }

    public void InvokeSwordFireAttack()
    {
        canAttack = true;

        if (!isAttacking_SwordFire && !PlayerStats.IsDead)
        {
            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            attackTask = new Task(AttackSwordFire());
        }
    }

    private IEnumerator AttackSwordFire()
    {
        if (canAttack && !Player.instance.takingDamage)
        {
            if (PlayerStats.IsDead)
            {
                canAttack = false;
                isAttacking_SwordFire = false;
                yield break;
            }

            isAttacking_SwordFire = true;

            // Play player sword fire attack animation
            animator.SetTrigger("SwordFireAttack");

            // Spawn magic particles
            new Task(SpawnFireParticles());

            // Wait for attack to end
            yield return new WaitForSeconds(swordFireAttackRepeatTime);
        }
        isAttacking_SwordFire = false;
    }

    private IEnumerator SpawnFireParticles()
    {
        yield return new WaitForSeconds(0.2f);

        // Wait for anim to reach fire particle spawn location
        yield return new WaitForSeconds(0.4f);

        // Spawn fire particles
        foreach (Transform child in fireParticlesSpawnLocations.transform)
        {
            // Play fireball spawn sound
            AudioManager.PlayOneShotSound(AudioManager.Sound.FireballSpawn);

            Instantiate(fireballPrefab, child.position, child.parent.localRotation);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void InvokeSwordBlock()
    {
        if (!isBlocking_Sword)
        {
            StopAttack();

            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            attackTask = new Task(SwordBlock());
        }
    }

    private IEnumerator SwordBlock()
    {
        if (PlayerStats.IsDead)
        {
            isBlocking_Sword = false;
            yield break;
        }

        Player.StopMovement();

        // Play player sword block animation
        animator.SetTrigger("SwordBlock");

        yield return new WaitForSeconds(0.2f);

        isBlocking_Sword = true;

        yield return new WaitForSeconds(0.3f);

        isBlocking_Sword = false;

        Player.EnableMovement();
    }

    public void TakeDamage(Transform enemyPos, int damageAmount)
    {
        if (!PlayerStats.IsDead && !isBlocking_Sword)
        {
            // Create blood particles
            GameObject bloodParticles = Instantiate(GameAssets.instance.bloodParticles, bloodParticlesStartPosition);

            if (Player.PlayerFacingRight())
            {
                bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                bloodParticles.transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            // Play hurt sound
            AudioManager.PlayPersistentSingleSoundIfNotAlreadyPlaying(AudioManager.Sound.PlayerGettingHit);

            // Set player taking damage bool on Player script to stop keybaord movement
            Player.instance.takingDamage = true;
            
            StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

            // Stop attack task
            StopAttack();

            // Reduce health
            PlayerStats.currentHealth -= damageAmount;

            // Push player in direction of hit
            StartCoroutine(PushPlayerInHitDirection(enemyPos));

            // Update Health Bar and display it
            healthBar.SetHealth(PlayerStats.currentHealth);

            // Play hurt animation
            animator.SetTrigger("Hurt");

            // Die if health is less than 0
            if (PlayerStats.currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator PushPlayerInHitDirection(Transform enemyPos)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (enemyPos.transform.position.x < transform.position.x)
        {
            // Push push to the right
            rb.velocity = new Vector2(pushDistanceOnHit, 0);
            yield return new WaitForSeconds(0.5f);
            Player.instance.takingDamage = false;
            canAttack = true;
        }
        else
        {
            // Push player to the left
            rb.velocity = new Vector2(-pushDistanceOnHit, 0);
            yield return new WaitForSeconds(0.5f);
            Player.instance.takingDamage = false;
            canAttack = true;
        }
    }

    void Die()
    {
        PlayerStats.IsDead = true;
        Player.StopMovement();
        PlayerStats.playerState = PlayerStats.PlayerState.Idle;

        // Stop attack task
        StopAttack();

        // Play death audio
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.PlayerDeath);

        // Set die animation parameter
        animator.SetBool("IsDead", true);

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        // Load game over screen after delay
        new Task(LoadGameOverScreenAfterDelay(3f));
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private IEnumerator LoadGameOverScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        LevelManager.SetAnimatorSpeed(0.4f);
        LevelManager.LoadSceneByName("GameOver");
    }
}