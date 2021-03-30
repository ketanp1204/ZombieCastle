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
        Zombie2
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
    public HealthBar healthBar;
    public Transform bloodParticlesStartPosition;
    public BoxCollider2D damageAreaCollider;
    public Animator attackHitboxAnimator;
    public GameObject attackHitbox;
    public BoxCollider2D hitboxCollider;

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
    public ZombieTypes zombieType;

    [HideInInspector]
    public bool isAttacking = false;

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

            attackHitbox.SetActive(false);
            // attackHitboxAnimator.SetBool("IsAttacking", false);
            isAttacking = false;
        }
    }

    public void InvokeAttack()
    {
        canAttack = true;
        animator = GetComponent<Animator>();

        if (isAttacking)
            return;

        if (enemyAI.enemyState == EnemyAI.EnemyState.Dead)
            return;

        if (PlayerStats.IsDead)
            return;

        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
            attackHitbox.SetActive(false);
        }

        attackTask = new Task(Attack());
    }

    private IEnumerator Attack()
    {
        if (enemyAI.enemyState == EnemyAI.EnemyState.Dead)
        {
            canAttack = false;
            isAttacking = false;
            yield break;
        }

        isAttacking = true;

        // Play attack sound if zombie 2 is attacking
        if (zombieType == ZombieTypes.Zombie2)
        {
            AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.Zombie2Attack);
        }

        // Play attack animation
        animator.SetTrigger("Attack");
        attackHitbox.SetActive(true);
        //attackHitboxAnimator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(attackRepeatTime);

        attackHitbox.SetActive(false);
        attackHitboxAnimator.SetBool("IsAttacking", false);
        
        isAttacking = false;
    }

    public void TakeDamage(Transform playerPos, int damageAmount)
    {
        if (enemyAI.enemyState == EnemyAI.EnemyState.Dead)
            return;

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
        if (zombieType == ZombieTypes.Zombie2)
        {
            if (Player.KnifeDrawn())
            {
                damageMultiplier = 0.05f;
            }
            else if (Player.AxeDrawn())
            {
                damageMultiplier = 3f;
            }
        }

        if (zombieType == ZombieTypes.Zombie1)
        {
            // Play zombie 1 getting hurt audio
            AudioManager.PlayOneShotSound(AudioManager.Sound.Zombie1GettingHit);
        }
        else if (zombieType == ZombieTypes.Zombie2)
        {
            // Play zombie 2 getting hurt audio
            AudioManager.PlayOneShotSound(AudioManager.Sound.Zombie2GettingHit);
        }

        // Reduce health
        currentHealth -= (int)Mathf.Floor(damageAmount * damageMultiplier);

        // Update Health Bar
        healthBar.SetHealth(currentHealth);

        // Push enemy in hit direction
        StartCoroutine(PushEnemyInHitDirection(playerPos));

        // Stop attack task
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        StopAttack();

        // Disable attack hitbox
        attackHitbox.SetActive(false);

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

        // Set enemy state to idle
        if (enemyAI.enemyState != EnemyAI.EnemyState.Dead)
            enemyAI.enemyState = EnemyAI.EnemyState.Idle;
    }

    void Die()
    {
        // Set enemy state to dead
        enemyAI.enemyState = EnemyAI.EnemyState.Dead;

        // Stop attack task
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        StopAttack();

        // Disable damage area collider
        damageAreaCollider.enabled = false;

        // Disable the hitbox collider
        hitboxCollider.enabled = false;

        // Set die animation parameter
        animator.SetBool("IsDead", true);

        // Play zombie death sound
        if (zombieType == ZombieTypes.Zombie1)
        {
            // Play zombie 1 getting hurt audio
            AudioManager.PlayOneShotSound(AudioManager.Sound.Zombie1Death);
        }
        else if (zombieType == ZombieTypes.Zombie2)
        {
            // Play zombie 2 getting hurt audio
            AudioManager.PlayOneShotSound(AudioManager.Sound.Zombie2Death);
        }

        // Stop pathfinding
        enemyAI.followPath = false;
        enemyAI.StopMovement();

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Destroy attack hitbox
        attackHitboxAnimator.enabled = false;
        Destroy(attackHitbox);

        // Set the enemy to layer 13 (DeadZombie) to prevent collision with player
        gameObject.layer = 13;

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        // Send a notification to the multiple zombies death behaviour script
        MultipleZombiesDeathBehaviour.instance.AddDeadZombie();

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
