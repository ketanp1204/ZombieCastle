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
    
    [Header("Axe Attack")]
    public int axeDamage = 20;                                                          // Float - Damage caused to enemy
    public float axeAttackRepeatTime = 1f;                                              // Float - Delay between successive attacks
    public Animator axeHitboxAnimator_L;                                                // Reference - Left hitbox animator
    public Animator axeHitboxAnimator_R;                                                // Reference - Right hitbox animator

    [Header("Knife Attack")]
    public int knifeDamage = 10;                                                        // Float - Damage caused to enemy
    public float knifeAttackRepeatTime = 1f;                                            // Float - Delay between successive attacks
    public Animator knifeHitboxAnimator_L;                                              // Reference - Left hitbox animator
    public Animator knifeHitboxAnimator_R;                                              // Reference - Right hitbox animator

    [Header("Common Attributes")]
    public LayerMask enemyLayers;                                                       // LayerMask - Enemies

    public float pushDistanceOnHit;                                                     // Float - Distance by which player is pushed back on taking damage

    [HideInInspector]
    public bool isAttacking_Axe = false;                                                // Bool - Axe attack status

    [HideInInspector]
    public bool isAttacking_Knife = false;                                              // Bool - Knife attack status

    [HideInInspector]
    public bool canAttack = false;                                                      // Bool - Player allowed to attack

    public event Action<PlayerCombat> OnDeath;                                          // Death Event

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
        canAttack = false;
    }

    public void InvokeAxeAttack()
    {
        canAttack = true;
        if (!isAttacking_Axe && !PlayerStats.IsDead)
        {
            isAttacking_Axe = true;
            StartCoroutine(AttackAxe());
        }
    }

    public void InvokeKnifeAttack()
    {
        canAttack = true;
        if (!isAttacking_Knife && !PlayerStats.IsDead)
        {
            isAttacking_Knife = true;
            StartCoroutine(AttackKnife());
        }
    }

    // TODO: Debug: Every second axe attack does not move the hitbox
    private IEnumerator AttackAxe()
    {
        if (Player.instance)
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
        else
        {
            bool facingRight = PlayerTopDown.PlayerFacingRight();
            if (canAttack && !PlayerTopDown.instance.takingDamage)
            {
                if (PlayerStats.IsDead)
                {
                    canAttack = false;
                    isAttacking_Axe = false;
                    yield break;
                }

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
    }

    public void PlayAxeAttackSound()
    {
        // Play axe attack sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerAxeAttack);
    }

    private IEnumerator AttackKnife()
    {
        if (Player.instance != null)
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
        else
        {
            bool facingRight = PlayerTopDown.PlayerFacingRight();
            if (canAttack && !PlayerTopDown.instance.takingDamage)
            {
                if (PlayerStats.IsDead)
                {
                    canAttack = false;
                    isAttacking_Knife = false;
                    yield break;
                }

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
    }

    public void PlayKnifeAttackSound()
    {
        // Play knife attack sound
        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.PlayerKnifeAttack);
    }

    public void TakeDamage(Transform enemyPos, int damage)
    {
        if (!PlayerStats.IsDead)
        {
            // Create blood particles
            GameObject bloodParticles = Instantiate(GameAssets.instance.bloodParticles, bloodParticlesStartPosition);

            if (Player.instance != null)
            {
                if (Player.PlayerFacingRight())
                {
                    bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    bloodParticles.transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }

                // Set player taking damage bool on Player script to stop keybaord movement
                Player.instance.takingDamage = true;
            }
            else
            {
                if (PlayerTopDown.PlayerFacingRight())
                {
                    bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    bloodParticles.transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }

                // Set player taking damage bool on Player script to stop keybaord movement
                PlayerTopDown.instance.takingDamage = true;
            }
            
            StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

            // Prevent player attack
            canAttack = false;

            // Reduce health
            PlayerStats.currentHealth -= damage;

            // Push player in direction of hit
            StartCoroutine(PushPlayerInHitDirection(enemyPos));

            // Update Health Bar and display it
            healthBar.SetHealth(PlayerStats.currentHealth);

            // Play hurt animation
            animator.SetTrigger("Hurt");

            // Stop attack hitbox animation
            axeHitboxAnimator_L.enabled = false;
            axeHitboxAnimator_R.enabled = false;
            knifeHitboxAnimator_L.enabled = false;
            knifeHitboxAnimator_R.enabled = false;

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
            if (Player.instance != null)
                Player.instance.takingDamage = false;
            else
                PlayerTopDown.instance.takingDamage = false;
            canAttack = true;
        }
        else
        {
            // Push player to the left
            rb.velocity = new Vector2(-pushDistanceOnHit, 0);
            yield return new WaitForSeconds(0.5f);
            if (Player.instance != null)
                Player.instance.takingDamage = false;
            else
                PlayerTopDown.instance.takingDamage = false;
            canAttack = true;
        }
    }

    void Die()
    {
        PlayerStats.IsDead = true;
        if (Player.instance != null)
        {
            Player.StopMovement();
        }
        else
        {
            PlayerTopDown.StopMovement();
        }
            
        // Play die animation
        animator.SetBool("IsDead", true);

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Disable Health Bar
        // healthBar.gameObject.SetActive(false);

        StartCoroutine(DestroyPlayerAndReturnToLobby(gameObject, 5f));
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private IEnumerator DestroyPlayerAndReturnToLobby(GameObject player, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(player);

        GameSession.ResetPlayerStats();
        SceneManager.LoadScene("CastleLobby");        // for testing, change later
    }
}