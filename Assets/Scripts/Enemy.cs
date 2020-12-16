using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Cached References
    public Animator animator;
    public HealthBar healthBar;

    // Variables
    public int maxHealth = 100;
    int currentHealth;
    private UnityEngine.Object enemyReference;
    public bool IsDead = false;

    // Death Event
    public event Action<Enemy> OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        enemyReference = Resources.Load(gameObject.name.Substring(0, 7));
        currentHealth = maxHealth;
        healthBar.gameObject.SetActive(true);
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        // Reduce health
        currentHealth -= damage;

        // Update Health Bar
        healthBar.SetHealth(currentHealth);

        // Play hurt animation
        animator.SetTrigger("Hurt");

        if(!IsDead)
        {
            // Die if health is less than 0
            if (currentHealth <= 0)
            {
                Die();
            }
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

        // Disable the enemy
        // gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        Invoke("Respawn", 4);

        StartCoroutine(DestroyGameObjectAfterDelay(gameObject));
    }

    void Respawn()
    {   
        GameObject enemyClone = (GameObject)Instantiate(enemyReference);
        enemyClone.transform.position = transform.position;
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
