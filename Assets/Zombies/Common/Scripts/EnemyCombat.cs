using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyCombat : MonoBehaviour
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
    private EnemyAI enemyAI;

    // Public references
    [Header("References")]
    public LayerMask playerLayerMask;
    public HealthBar healthBar;
    public Transform bloodParticlesStartPosition;
    public Animator attackHitboxAnimator;
    public GameObject attackHitbox;
    public BoxCollider2D hitboxCollider;

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
    public event Action<EnemyCombat> OnDeath;

    private void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        enemyAI = GetComponent<EnemyAI>();
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

        if (enemyAI.enemyState != EnemyAI.EnemyState.Attacking && enemyAI.enemyState != EnemyAI.EnemyState.Dead)
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
            enemyAI.enemyState = EnemyAI.EnemyState.Attacking;

            if (enemyAI.enemyState == EnemyAI.EnemyState.Dead)
            {
                canAttack = false;
                yield break;
            }

            // Play attack animation
            animator.SetTrigger("Attack");
            attackHitboxAnimator.enabled = true;
            attackHitboxAnimator.SetBool("IsAttacking", true);

            yield return new WaitForSeconds(attackRepeatTime);
        }
        enemyAI.enemyState = EnemyAI.EnemyState.Chasing;
    }

    public void TakeDamage(Transform playerPos, int damageAmount)
    {
        if (enemyAI.enemyState != EnemyAI.EnemyState.Dead)
        {
            // Create blood particles
            GameObject bloodParticles = Instantiate(GameAssets.instance.bloodParticles, bloodParticlesStartPosition);

            if (enemyAI.facingRight)
            {
                bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                bloodParticles.transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

            // Set damage multiplier
            if (Player.KnifeDrawn() && zombieType != ZombieTypes.Zombie1)
            {
                damageMultiplier = 0.05f;
            }
            else if (Player.AxeDrawn())
            {
                damageMultiplier = 0.8f;
            }

            // Reduce health
            currentHealth -= (int)Mathf.Floor(damageAmount * damageMultiplier);

            // Update Health Bar
            healthBar.SetHealth(currentHealth);

            // Push enemy in hit direction
            StartCoroutine(PushEnemyInHitDirection(playerPos));

            // Stop attack task
            StopAttack();

            // Play hurt animation
            animator.SetTrigger("Hurt");

            // Stop attack hitbox animation if running
            // attackHitboxAnimator.enabled = false;
            attackHitboxAnimator.SetBool("IsAttacking", false);

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
        enemyAI.enemyState = EnemyAI.EnemyState.TakingDamage;

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
        enemyAI.enemyState = EnemyAI.EnemyState.Chasing;
    }

    void Die()
    {
        // Set enemy state to dead
        enemyAI.enemyState = EnemyAI.EnemyState.Dead;

        // Stop attack task
        StopAttack();

        // Set die animation parameter
        animator.SetBool("IsDead", true);

        // Stop pathfinding
        enemyAI.followPath = false;
        enemyAI.StopMovement();

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Set the enemy to layer 13 (DeadZombie) to prevent collision with player
        gameObject.layer = 13;

        // Disable the hitbox collider
        hitboxCollider.enabled = false;

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        // Send a notification to the multiple zombies death behaviour script
        MultipleZombiesDeathBehaviour.instance.AddDeadZombie();

        enemyAI.enemyState = EnemyAI.EnemyState.Dead;

        // Destroy enemy after delay
        StartCoroutine(DestroyGameObjectAfterDelay(gameObject, destroyDelayAfterDeath));
    }

    // Utilities

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
