using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(EnemyCombat))]
public class Enemy : MonoBehaviour
{
    // Cached References
    private Animator animator;
    public HealthBar healthBar;

    // Variables
    public int maxHealth;
    int currentHealth;
    private UnityEngine.Object enemyReference;
    [HideInInspector]
    public bool IsDead = false;
    public float destroyDelayAfterDeath = 5f;

    // Start is called before the first frame update
    void Start()
    {
        enemyReference = Resources.Load(gameObject.name.Substring(0, 7));
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.gameObject.SetActive(true);
        healthBar.SetMaxHealth(maxHealth);
    }
}
