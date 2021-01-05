using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack 1 Attributes")]
    public Transform attack1LeftPoint;      // Location of left attack point
    public Transform attack1RightPoint;     // Location of right attack point
    public float attack1Range;              // Range at which attack is enabled
    public int attack1Damage = 20;          // Damage caused to enemy

    [Header("Attack 2 Attributes")]
    public Transform attack2LeftPoint;      // Location of left attack point
    public Transform attack2RightPoint;     // Location of right attack point
    public float attack2Range;              // Range at which attack is enabled
    public int attack2Damage = 50;          // Damage caused to enemy

    [Header("Common Attributes")]
    public LayerMask enemyLayers;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Player.instance.weaponDrawn)
        {
            Attack1();
        }

        /*
        if (Input.GetMouseButtonDown(1) && Player.instance.weaponDrawn)
        {
            Attack2();
        }
        */
    }

    void Attack1()
    {
        AudioManager.PlaySound(AudioManager.Sound.PlayerAxeHit, transform.position);
        Collider2D[] hitEnemies;

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
        

        // Damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            if(!enemy.transform.parent.gameObject.GetComponent<Enemy>().IsDead)
            {
                enemy.transform.parent.gameObject.GetComponent<Enemy>().TakeDamage(attack1Damage);
                enemy.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }

    }

    void Attack2()
    {
        // Detect enemies in range of attack
        Collider2D[] hitEnemies;

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

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            if(!enemy.transform.parent.gameObject.GetComponent<Enemy>().IsDead)
            {
                enemy.transform.parent.gameObject.GetComponent<Enemy>().TakeDamage(attack2Damage);
                enemy.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }
}
