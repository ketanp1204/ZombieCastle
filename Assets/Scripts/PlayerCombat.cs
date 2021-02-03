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

    [Header("Axe Attack")]
    public int axeDamage = 20;                                                          // Damage caused to enemy
    public float axeAttackRepeatTime = 1f;                                              // Delay between successive attacks
    public GameObject axeHitbox_L;                                                      // GameObject of the left hitbox of the axe
    public GameObject axeHitbox_R;                                                      // GameObject of the right hitbox of the axe
    public Animator axeHitboxAnimator_L;                                                // Reference to the animator of the left hitbox
    public Animator axeHitboxAnimator_R;                                                // Reference to the animator of the right hitbox

    [Header("Knife Attack")]
    public int knifeDamage = 10;                                                        // Damage caused to enemy
    public float knifeAttackRepeatTime = 1f;                                            // Delay between successive attacks
    public GameObject knifeHitbox_L;                                                    // GameObject of the left hitbox of the knife
    public GameObject knifeHitbox_R;                                                    // GameObject of the left hitbox of the knife
    public Animator knifeHitboxAnimator_L;                                              // Reference to the animator of the left hitbox
    public Animator knifeHitboxAnimator_R;                                              // Reference to the animator of the right hitbox

    [Header("Common Attributes")]
    public LayerMask enemyLayers;
    public float pushDistanceOnHit;                                                     // Distance by which player is pushed back on taking damage
    [HideInInspector]
    public bool isAttacking_Axe = false;
    [HideInInspector]
    public bool isAttacking_Knife = false;
    [HideInInspector]
    public bool canAttack = false;

    // Death Event
    public event Action<PlayerCombat> OnDeath;

    // Cached References
    public HealthBar healthBar;
    public Transform bloodParticlesStartPosition;
    private SceneType sceneTypeReference;
    private Animator animator;

    // Private variables
    private int sceneType;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        sceneTypeReference = FindObjectOfType<SceneType>();
        animator = GetComponent<Animator>();

        if (sceneTypeReference.type == SceneType.SceneTypes.S_TwoD)
        {
            sceneType = 1;
        }
        else
        {
            sceneType = 2;
        }

        knifeHitbox_L.SetActive(false);
    }

    public void StopAttack()
    {
        canAttack = false;
    }

    /*
    public void InvokeAxeAttack()
    {
        AxeAttack();
    }
    
    public void InvokeKnifeAttack()
    {
        KnifeAttack();
    }
    */

    public void InvokeAxeAttack()
    {
        canAttack = true;
        if (!isAttacking_Axe && !Player.instance.IsDead)
        {
            isAttacking_Axe = true;
            StartCoroutine(AttackAxe());
        }
    }

    public void InvokeKnifeAttack()
    {
        canAttack = true;
        if (!isAttacking_Knife && !Player.instance.IsDead)
        {
            isAttacking_Knife = true;
            StartCoroutine(AttackKnife());
        }
    }

    /*
    private void AxeAttack()
    {
        // Play axe hit sound
        AudioManager.PlaySoundAtPosition(AudioManager.Sound.PlayerAxeHit, transform.position);

        // Detect enemies in range of attack
        Collider2D[] hitEnemies;

        if (sceneType == 1)
        {
            if (Player.PlayerFacingRight() == true)
            {
                // Detect enemies in range of right attack position
                hitEnemies = Physics2D.OverlapCircleAll(attack1RightPoint.position, attack1Range, enemyLayers);
            }
            else
            {
                // Detect enemies in range of left attack position
                hitEnemies = Physics2D.OverlapCircleAll(attack1LeftPoint.position, attack1Range, enemyLayers);
            }
        }
        else
        {
            if (PlayerTopDown.PlayerFacingRight() == true)
            {
                // Detect enemies in range of right attack position
                hitEnemies = Physics2D.OverlapCircleAll(attack1RightPoint.position, attack1Range, enemyLayers);
            }
            else
            {
                // Detect enemies in range of left attack position
                hitEnemies = Physics2D.OverlapCircleAll(attack1LeftPoint.position, attack1Range, enemyLayers);
            }
        }
        
        // Damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            if(!enemy.transform.parent.gameObject.GetComponent<EnemyCombat>().IsDead)
            {
                enemy.transform.parent.gameObject.GetComponent<EnemyCombat>().TakeDamage(Player.instance.pathfindingTarget, attack1Damage);
                // enemy.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }

    }
    */

    private IEnumerator AttackAxe()
    {
        bool facingRight = Player.PlayerFacingRight();
        if (canAttack && !Player.instance.takingDamage)
        {
            if (Player.instance.IsDead)
            {
                canAttack = false;
                isAttacking_Axe = false;
                yield break;
            }

            // Enable hitbox
            if (facingRight)
            {
                axeHitbox_R.SetActive(true);
                axeHitboxAnimator_R.SetTrigger("Attack");
            }
            else
            {
                axeHitbox_L.SetActive(true);
                axeHitboxAnimator_L.SetTrigger("Attack");
            }

            // Wait for attack to end
            yield return new WaitForSeconds(axeAttackRepeatTime);
        }
        isAttacking_Axe = false;
        if (facingRight)
        {
            axeHitbox_R.SetActive(false);
        }
        else
        {
            axeHitbox_L.SetActive(false);
        }
    }

    private IEnumerator AttackKnife()
    {
        bool facingRight = Player.PlayerFacingRight();
        if (canAttack && !Player.instance.takingDamage)
        {
            if (Player.instance.IsDead)
            {
                canAttack = false;
                isAttacking_Knife = false;
                yield break;
            }

            // Enable hitbox and play animation
            if (facingRight)
            {
                knifeHitbox_R.SetActive(true);
                knifeHitboxAnimator_R.SetTrigger("Attack");
            }
            else
            {
                knifeHitbox_L.SetActive(true);
                knifeHitboxAnimator_L.SetTrigger("Attack");
            }

            // Wait for attack to end
            yield return new WaitForSeconds(knifeAttackRepeatTime);
        }
        isAttacking_Knife = false;
        if (facingRight)
        {
            knifeHitbox_R.SetActive(false);
        }
        else
        {
            knifeHitbox_L.SetActive(false);
        }
    }

    /*
    void KnifeAttack()
    {
        // TODO: Play knife hit sound

        // Enable knife attack hitbox
        knifeAttackHitboxGO.SetActive(true);

        // Detect enemies in range of attack
        Collider2D[] hitEnemies;

        if (sceneType == 1)
        {
            if (Player.PlayerFacingRight() == true)
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(knifeAttackHitbox_R.position, knifeAttackRange, enemyLayers);
            }
            else
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(knifeAttackHitbox_L.position, knifeAttackRange, enemyLayers);
            }
        }
        else
        {
            if (PlayerTopDown.PlayerFacingRight() == true)
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(knifeAttackHitbox_R.position, knifeAttackRange, enemyLayers);
            }
            else
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(knifeAttackHitbox_L.position, knifeAttackRange, enemyLayers);
            }
        }
        

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            if(!enemy.transform.parent.gameObject.GetComponent<EnemyCombat>().IsDead)
            {
                enemy.transform.parent.gameObject.GetComponent<EnemyCombat>().TakeDamage(Player.instance.pathfindingTarget, knifeDamage);
                // enemy.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }
    */

    public void TakeDamage(Transform enemyPos, int damage)
    {
        if (!PlayerStats.IsDead)
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
            StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

            // Prevent player attack
            canAttack = false;

            // Set player taking damage bool on Player script to stop keybaord movement
            Player.instance.takingDamage = true;

            // Reduce health
            PlayerStats.currentHealth -= damage;

            // Push player in direction of hit
            StartCoroutine(PushPlayerInHitDirection(enemyPos));

            // Update Health Bar
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

        // Play die animation
        animator.SetBool("IsDead", true);

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

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

        PlayerStats.isFirstScene = true;
        SceneManager.LoadScene("CastleLobby");
    }
}
