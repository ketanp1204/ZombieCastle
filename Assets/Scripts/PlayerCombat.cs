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

    [Header("Attack 1 Attributes")]
    public Transform attack1LeftPoint;      // Location of left attack point
    public Transform attack1RightPoint;     // Location of right attack point
    public float attack1Range;              // Range at which attack is enabled
    public int attack1Damage = 20;          // Damage caused to enemy
    public float attack1RepeatTime;         // Delay between successive attacks

    [Header("Attack 2 Attributes")]
    public Transform attack2LeftPoint;      // Location of left attack point
    public Transform attack2RightPoint;     // Location of right attack point
    public float attack2Range;              // Range at which attack is enabled
    public int attack2Damage = 50;          // Damage caused to enemy
    public float attack2RepeatTime;         // Delay between successive attacks

    [Header("Common Attributes")]
    public LayerMask enemyLayers;
    public float pushDistanceOnHit;         // Distance by which player is pushed back on taking damage

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
    }

    public void InvokeAxeAttack()
    {
        AxeAttack();
    }

    public void InvokeKnifeAttack()
    {
        KnifeAttack();
    }

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

    void KnifeAttack()
    {
        // TODO: Play knife hit sound


        // Detect enemies in range of attack
        Collider2D[] hitEnemies;

        if (sceneType == 1)
        {
            if (Player.PlayerFacingRight() == true)
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(attack2RightPoint.position, attack2Range, enemyLayers);
            }
            else
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(attack2LeftPoint.position, attack2Range, enemyLayers);
            }
        }
        else
        {
            if (PlayerTopDown.PlayerFacingRight() == true)
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(attack2RightPoint.position, attack2Range, enemyLayers);
            }
            else
            {
                // Detect enemies in range of attack
                hitEnemies = Physics2D.OverlapCircleAll(attack2LeftPoint.position, attack2Range, enemyLayers);
            }
        }
        

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            if(!enemy.transform.parent.gameObject.GetComponent<EnemyCombat>().IsDead)
            {
                enemy.transform.parent.gameObject.GetComponent<EnemyCombat>().TakeDamage(Player.instance.pathfindingTarget, attack2Damage);
                // enemy.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

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

            // Set player taking damage bool on Player script to stop keybaord movement and attacks
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
        }
        else
        {
            // Push player to the left
            rb.velocity = new Vector2(-pushDistanceOnHit, 0);
            yield return new WaitForSeconds(0.5f);
            Player.instance.takingDamage = false;
        }
    }

    void Die()
    {
        PlayerStats.IsDead = true;
        Player.instance.movePlayer = false;

        // Play die animation
        animator.SetBool("IsDead", true);

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        StartCoroutine(DestroyGameObjectAfterDelay(gameObject, 5f));
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);

        PlayerStats.isFirstScene = true;
        SceneManager.LoadScene("CastleLobby");
    }
}
