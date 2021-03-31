using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(RangedEnemyAI))]
public class RangedEnemyCombat : MonoBehaviour
{
    // Public attack variables
    [Header("Magic Attack")]
    public float magicAttackRepeatTime = 0.8f;                                          // Float - Delay between successive attacks
    public Transform magicParticlesSpawnLocation;                                       // Transform - Position from which to spawn the magic spell particles
    public GameObject magicParticlePrefab;                                              // GameObject - Prefab of the zombie3 magic spell moving particle

    [HideInInspector]
    public bool canAttack = false;

    // Private references
    private Animator animator;
    private RangedEnemyAI rangedEnemyAI;
    private BoxCollider2D damageCollider;

    // Public references
    [Header("References")]
    public HealthBar healthBar;
    public Transform bloodParticlesStartPosition;

    // Private general variables
    private int currentHealth;
    private Task attackTask = null;
    private float healthWhenAttackStarted;

    // Public general variables
    [Header("Enemy Attributes")]
    public int maxHealth;
    public float destroyDelayAfterDeath = 3f;
    public float pushDistanceOnHit;
    public float damageMultiplier = 1f;

    [HideInInspector]
    public bool isAttacking = false;

    // Death Event
    public event Action<RangedEnemyCombat> OnDeath;


    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        rangedEnemyAI = GetComponent<RangedEnemyAI>();
        animator = GetComponent<Animator>();
        damageCollider = transform.Find("DamageArea").GetComponent<BoxCollider2D>();
    }

    private void Initialize()
    {
        currentHealth = maxHealth;
        healthBar.gameObject.SetActive(true);
        healthBar.SetMaxHealth(maxHealth);
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

            isAttacking = false;
        }
    }

    public void InvokeAttack()
    {
        canAttack = true;
        animator = GetComponent<Animator>();

        if (isAttacking)
            return;

        if (rangedEnemyAI.enemyState == RangedEnemyAI.EnemyState.Dead)
            return;

        if (PlayerStats.IsDead)
            return;


        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        attackTask = new Task(Attack());
    }

    private IEnumerator Attack()
    {
        if (canAttack)
        {
            if (rangedEnemyAI.enemyState == RangedEnemyAI.EnemyState.Dead)
            {
                StopAttack();
                canAttack = false;
                isAttacking = false;
                yield break;
            }

            isAttacking = true;

            // Play attack sound
            AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.Zombie3Attack);

            // Play attack animation
            animator.SetTrigger("Attack");

            // Spawn magic particles
            new Task(SpawnMagicParticles());

            yield return new WaitForSeconds(magicAttackRepeatTime);

            isAttacking = false;
        }
    }

    private IEnumerator SpawnMagicParticles()
    {
        // Wait for anim to reach magic particle spawn location
        yield return new WaitForSeconds(0.2f);

        if (isAttacking)
        {
            // Spawn magic particles
            Instantiate(magicParticlePrefab, magicParticlesSpawnLocation.position, magicParticlesSpawnLocation.localRotation);

            yield return new WaitForSeconds(0.1f);

            Instantiate(magicParticlePrefab, magicParticlesSpawnLocation.position, magicParticlesSpawnLocation.localRotation);
        }
    }

    public void TakeDamage(Transform playerPosition, int damageAmount)
    {
        if (rangedEnemyAI.enemyState == RangedEnemyAI.EnemyState.Dead)
            return;

        if (!rangedEnemyAI.combatStarted)
            return;

        // Create blood particles
        GameObject bloodParticles = Instantiate(GameAssets.instance.bloodParticles, bloodParticlesStartPosition);

        if (rangedEnemyAI.facingRight)
        {
            bloodParticles.transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

        // Play zombie 2 getting hurt audio for zombie 3 (unless audio changed later)
        AudioManager.PlayOneShotSound(AudioManager.Sound.Zombie3GettingHit);

        // Reduce health
        currentHealth -= (int)Mathf.Floor(damageAmount * damageMultiplier);

        // Update Health Bar
        healthBar.SetHealth(currentHealth);

        // Push enemy in hit direction
        StartCoroutine(PushEnemyInHitDirection(playerPosition));

        // Stop attack task
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        StopAttack();

        // Play hurt animation
        animator.SetTrigger("Hurt");

        // Die if health is less than 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator PushEnemyInHitDirection(Transform playerPos)
    {
        // Get rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Set enemy state to taking damage
        rangedEnemyAI.enemyState = RangedEnemyAI.EnemyState.TakingDamage;

        if (playerPos.position.x < transform.position.x)
        {
            // Push enemy to the right
            rb.velocity = new Vector2(pushDistanceOnHit, rb.velocity.y);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            // Push enemy to the left
            rb.velocity = new Vector2(-pushDistanceOnHit, rb.velocity.y);
            yield return new WaitForSeconds(0.5f);
        }

        // Set enemy state to idle
        if (rangedEnemyAI.enemyState != RangedEnemyAI.EnemyState.Dead)
            rangedEnemyAI.enemyState = RangedEnemyAI.EnemyState.Idle;
    }

    void Die()
    {
        // Stop pathfinding
        rangedEnemyAI.followPath = false;
        rangedEnemyAI.StopMovement();

        // Set enemy state to dead
        rangedEnemyAI.enemyState = RangedEnemyAI.EnemyState.Dead;

        // Stop attack task
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        StopAttack();

        // Disable damage collider
        damageCollider.enabled = false;

        // Set die animation parameter
        animator.SetBool("IsDead", true);

        // Play zombie 3 death sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.Zombie3Death);

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Set the enemy to layer 13 (DeadZombie) to prevent collision with player
        gameObject.layer = 13;

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        // Send a notification to the multiple zombies death behaviour script
        MultipleZombiesDeathBehaviour.instance.AddDeadZombie();

        // Destroy enemy after delay
        StartCoroutine(DestroyGameObjectAfterDelay(gameObject, destroyDelayAfterDeath));
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
