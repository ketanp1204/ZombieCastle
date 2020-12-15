using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack 1 Attributes")]
    public Transform attack1Point;      // Location of point which registers attack
    public float attack1Range;          // Range at which attack is enabled
    public int attack1Damage = 20;      // Damage caused to enemy

    [Header("Attack 2 Attributes")]
    public Transform attack2Point;      // Location of point which registers attack
    public float attack2Range;          // Range at which attack is enabled
    public int attack2Damage = 50;      // Damage caused to enemy

    [Header("Common Attributes")]
    public LayerMask enemyLayers;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack1();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Attack2();
        }
    }

    void Attack1()
    {
        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attack1Point.position, attack1Range, enemyLayers);

        // Damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attack1Damage);
            enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

    }

    void Attack2()
    {
        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attack2Point.position, attack2Range, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attack2Damage);
            enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attack1Point == null)
        {
            Gizmos.DrawWireSphere(attack1Point.position, attack1Range);
        }
    }
}
