using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public enum AttackTypes
    {
        Attack1,
        Attack2
    }

    // Public attack attributes
    [Header("Attack 1 Attributes")]
    public int attack1Damage;
    public float attack1RepeatTime;

    [Header("Attack 2 Attributes")]
    public int attack2Damage;
    public float attack2RepeatTime;

    [HideInInspector]
    public bool canAttack = false;

    // Private references
    private Animator animator;
    private BossAI bossAI;

    // Public references
    [Header("References")]
    public LayerMask playerLayerMask;
    public HealthBar healthBar;
    public Transform bloodParticlesStartPosition;

    // Private general variables
    private int currentHealth;
    private Task attackTask = null;

    // Public general variables
    [Header("Enemy Attributes")]
    public int maxHealth;
    public float destroyDelayAfterDeath = 3f;
    public float pushDistanceOnHit;
    public float damageMultiplier = 1f;

    [HideInInspector]
    public AttackTypes currentAttackType = AttackTypes.Attack1;

    // Death Event
    public Action<BossCombat> OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        bossAI = GetComponent<BossAI>();
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

        if (bossAI.state != BossAI.BossState.Attacking && bossAI.state != BossAI.BossState.Dead)
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
            bossAI.state = BossAI.BossState.Attacking;

            if (bossAI.state == BossAI.BossState.Dead)
            {
                canAttack = false;
                yield break;
            }

            if (currentAttackType == AttackTypes.Attack1)
            {
                // Play attack 1 animation
                animator.SetTrigger("Attack1");

                yield return new WaitForSeconds(attack1RepeatTime);
            }
            else
            {
                // Play attack 2 animation
                animator.SetTrigger("Attack2");

                yield return new WaitForSeconds(attack2RepeatTime);
            }
        }
        bossAI.state = BossAI.BossState.Chasing;
    }

    public void TakeDamage(int damageAmount)
    {
        if (bossAI.state != BossAI.BossState.Dead)
        {
            // Create blood particles
            GameObject bloodParticles = Instantiate(GameAssets.instance.bloodParticles, bloodParticlesStartPosition);
            bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

            // TODO: set damage multiplier

            // Reduce health
            currentHealth -= (int)Mathf.Floor(damageAmount * damageMultiplier);

            // Update Health Bar
            healthBar.SetHealth(currentHealth);

            // Push boss in hit direction
            StartCoroutine(PushBossInHitDirection());

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

    private IEnumerator PushBossInHitDirection()
    {
        // Get rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Set boss state to taking damage
        bossAI.state = BossAI.BossState.TakingDamage;

        // Push boss to the left
        rb.velocity = new Vector2(-pushDistanceOnHit, rb.velocity.y);
        yield return new WaitForSeconds(0.5f);

        // Set boss state to chasing
        bossAI.state = BossAI.BossState.Chasing;
    }

    public void Die()
    {
        // Set boss state to dead
        bossAI.state = BossAI.BossState.Dead;

        // Stop attack task
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        // Set die animation parameter
        animator.SetBool("IsDead", true);

        // Stop pathfinding
        bossAI.followPath = false;
        bossAI.StopMovement();

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Set the boss to layer 13 (DeadZombie) to prevent collision with player
        gameObject.layer = 13;

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        // Notification to game ending script
        GameObject.Find("BehaviourAfterBossDefeated").GetComponent<BehaviourAfterBossDefeated>().StartGameEndBehaviour();
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
