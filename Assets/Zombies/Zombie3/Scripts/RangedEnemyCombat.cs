using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(RangedEnemyAI))]
public class RangedEnemyCombat : MonoBehaviour
{
    public enum ZombieTypes
    {
        Zombie1,
        Zombie2,
        Zombie3,
        Zombie4
    }

    // Public attack variables
    [Header("Attack Attributes")]
    public int attackDamage;
    public float attackRepeatTime;

    [HideInInspector]
    public bool canAttack = false;

    // Private references
    private Animator animator;
    private RangedEnemyAI rangedEnemyAI;

    // Public references
    [Header("References")]
    public LayerMask playerLayerMask;
    public HealthBar healthBar;
    public Transform bloodParticlesStartPosition;
    public Transform attackParticleSpawnLocation;

    // Private general variables
    private int currentHealth;
    private Task attackTask = null;

    // Public general variables
    [Header("Enemy Attributes")]
    public int maxHealth;
    public float destroyDelayAfterDeath = 3f;
    public float pushDistanceOnHit;
    public float damageMultiplier = 1f;
    public ZombieTypes zombieType;

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
        }
    }

    public void InvokeAttack()
    {
        canAttack = true;
        animator = GetComponent<Animator>();

        if (rangedEnemyAI.enemyState != RangedEnemyAI.EnemyState.Attacking && rangedEnemyAI.enemyState != RangedEnemyAI.EnemyState.Dead)
        {
            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }

            attackTask = new Task(Attack());
        }
    }

    private IEnumerator Attack()
    {
        float healthWhenAttackStarted = currentHealth;

        while (canAttack && healthWhenAttackStarted == currentHealth)
        {
            rangedEnemyAI.enemyState = RangedEnemyAI.EnemyState.Attacking;

            if (rangedEnemyAI.enemyState == RangedEnemyAI.EnemyState.Dead)
            {
                canAttack = false;
                yield break;
            }

            // Play attack animation
            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackRepeatTime);
        }
        rangedEnemyAI.enemyState = RangedEnemyAI.EnemyState.Chasing;
    }

    public void TakeDamage(Transform playerPosition, int damageAmount)
    {
        if (rangedEnemyAI.enemyState != RangedEnemyAI.EnemyState.Dead)
        {
            // Create blood particles
            GameObject bloodParticles = Instantiate(GameAssets.instance.bloodParticles, bloodParticlesStartPosition);

            if (rangedEnemyAI.facingRight)
            {
                bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                bloodParticles.transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

            // TODO: set damage multiplier

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

            // Play hurt animation
            animator.SetTrigger("Hurt");

            // Die if health is less than 0
            if (currentHealth <= 0)
            {
                Die();
            }

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

        // Set enemy state to chasing
        rangedEnemyAI.enemyState = RangedEnemyAI.EnemyState.Chasing;
    }

    void Die()
    {
        // Set enemy state to dead
        rangedEnemyAI.enemyState = RangedEnemyAI.EnemyState.Dead;

        // Stop attack task
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        // Set die animation parameter
        animator.SetBool("IsDead", true);

        // Stop pathfinding
        rangedEnemyAI.followPath = false;
        rangedEnemyAI.StopMovement();

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
