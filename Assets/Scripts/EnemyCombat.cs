using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(EnemyCombat))]
public class EnemyCombat : MonoBehaviour
{
    [Header("Attack 1 Attributes")]
    [HideInInspector]
    public bool attack1Trigger;
    [HideInInspector]
    public bool canAttack = false;
    [HideInInspector]
    public bool isAttacking = false;
    public Transform attack1Point;      // Location of point which registers attack
    public float attack1Range;          // Range at which attack is enabled
    public int attack1Damage = 20;      // Damage caused to enemy
    public float attackRepeatTime;      // Attack rate of enemy

    // Cached References
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
    private UnityEngine.Object enemyReference;
    private int currentHealth;

    // Public variables exposed to Inspector
    [Header("Enemy Attributes")]
    public int maxHealth;
    public float destroyDelayAfterDeath = 7f;
    public float pushDistanceOnHit;

    // Public variables hidden from Inspector
    [HideInInspector]
    public bool IsDead = false;
    [HideInInspector]
    public bool takingDamage = false;

    // Death Event
    public event Action<EnemyCombat> OnDeath;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyReference = Resources.Load(gameObject.name.Substring(0, 7));
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.gameObject.SetActive(true);
        healthBar.SetMaxHealth(maxHealth);
        attackHitbox.SetActive(false);
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
        while (canAttack && !takingDamage)
        {
            if (IsDead)
            {
                canAttack = false;
                isAttacking = false;
                yield break;
            }

            // Enable hitbox
            attackHitbox.SetActive(true);

            // Play attack animation
            animator.SetTrigger("Attack");
            attackHitboxAnimator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackRepeatTime);
        }
        isAttacking = false;
        attackHitbox.SetActive(false);
    }

    void Attack1()
    {
        // Play attack animation
        animator.SetTrigger("Attack");

        // Detect player in range of attack
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attack1Point.position, attack1Range, playerLayerMask);

        // Damage them
        foreach (Collider2D player in hitPlayer)
        {
            player.transform.parent.GetComponent<PlayerCombat>().TakeDamage(this.transform, attack1Damage);
        }
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

            // Reduce health
            currentHealth -= damage;

            // Update Health Bar
            healthBar.SetHealth(currentHealth);

            StartCoroutine(PushEnemyInHitDirection(playerPos));

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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

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

        // Invoke("Respawn", 4);

        StartCoroutine(DestroyGameObjectAfterDelay(gameObject, destroyDelayAfterDeath));
    }

    void Respawn()
    {
        GameObject enemyClone = (GameObject)Instantiate(enemyReference);
        enemyClone.transform.position = transform.position;
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
