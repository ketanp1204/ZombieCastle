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

    [Header("Attack 1 Attributes")]
    [HideInInspector]
    public bool canAttack = false;
    [HideInInspector]
    public bool isAttacking = false;
    public int attack1Damage = 20;      // Damage caused to enemy
    public float attackRepeatTime;      // Attack rate of enemy

    // Private References
    private Animator animator;
    private EnemyAI enemyAI;

    [Header("References")]
    public LayerMask playerLayerMask;
    public HealthBar healthBar;
    public Animator attackHitboxAnimator;
    public GameObject attackHitbox;
    public BoxCollider2D hitboxCollider;
    public Transform bloodParticlesStartPosition;

    // Private Variables
    private int currentHealth;

    // Public variables
    [Header("Enemy Attributes")]
    public int maxHealth;
    public float destroyDelayAfterDeath = 3f;
    public float pushDistanceOnHit;
    public float damageMultiplier = 1f;

    public ZombieTypes zombieType;

    // Public variables hidden from Inspector
    [HideInInspector]
    public bool IsDead = false;
    [HideInInspector]
    public bool takingDamage = false;

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
        canAttack = false;
    }

    public void InvokeAttack()
    {
        canAttack = true;
        animator = GetComponent<Animator>();

        if (!isAttacking && !IsDead)
        {
            isAttacking = true;
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        float healthWhenAttackStarted = currentHealth;

        while (canAttack && healthWhenAttackStarted == currentHealth)
        {
            if (IsDead)
            {
                canAttack = false;
                isAttacking = false;
                yield break;
            }

            // Play attack animation
            animator.SetTrigger("Attack");
            attackHitboxAnimator.enabled = true;
            //attackHitboxAnimator.SetTrigger("Attack");
            attackHitboxAnimator.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(attackRepeatTime);
        }
        isAttacking = false;
    }

    public void TakeDamage(Transform playerPos, int damage)
    {
        if (!IsDead)
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

            // Set enemy taking damage bool to true to stop attacks
            takingDamage = true;

            if (Player.KnifeDrawn() && zombieType != ZombieTypes.Zombie1)
            {
                damageMultiplier = 0.05f;
            }
            else if (Player.AxeDrawn())
            {
                damageMultiplier = 0.8f;
            }

            // Reduce health
            currentHealth -= (int)Mathf.Floor(damage * damageMultiplier);

            // Update Health Bar
            healthBar.SetHealth(currentHealth);

            // Push enemy in hit direction
            StartCoroutine(PushEnemyInHitDirection(playerPos));

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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        enemyAI.enemyState = EnemyAI.EnemyState.TakingDamage;

        if (playerPos.position.x < transform.position.x)
        {
            // Push enemy to the right
            rb.velocity = new Vector2(pushDistanceOnHit, rb.velocity.y);
            yield return new WaitForSeconds(0.5f);
            takingDamage = false;
        }
        else
        {
            // Push enemy to the left
            rb.velocity = new Vector2(-pushDistanceOnHit, rb.velocity.y);
            yield return new WaitForSeconds(0.5f);
            takingDamage = false;
        }

        enemyAI.enemyState = EnemyAI.EnemyState.Attacking;
    }

    void Die()
    {
        IsDead = true;

        // Play die animation
        animator.SetBool("IsDead", true);

        // Stop pathfinding
        EnemyAI enemyAI = GetComponent<EnemyAI>();
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

        StartCoroutine(DestroyGameObjectAfterDelay(gameObject, destroyDelayAfterDeath));
    }

    // Utilities

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
