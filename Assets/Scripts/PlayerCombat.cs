using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attack1Point;
    public float attack1Range = 0.5f;
    public int attack1Damage = 20;

    public Transform attack2Point;
    public float attack2Range = 0.5f;
    public int attack2Damage = 50;

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
