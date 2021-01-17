using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack 1 Attributes")]
    [HideInInspector]
    public bool attack1Trigger;
    [HideInInspector]
    public bool canAttack = false;
    public Transform attack1Point;      // Location of point which registers attack
    public float attack1Range;          // Range at which attack is enabled
    public int attack1Damage = 20;      // Damage caused to enemy
    public float attackRepeatTime;      // Attack rate of enemy

    // Cached References
    private Animator animator;
    public LayerMask playerLayerMask;

    public void StopAttack()
    {
        canAttack = false;
        // CancelInvoke();
    }

    public void InvokeAttack()
    {
        canAttack = true;
        StartCoroutine(Attack());
        // InvokeRepeating("Attack1", 0f, attackRepeatTime);
        animator = GetComponent<Animator>();
    }

    private IEnumerator Attack()
    {
        while(canAttack)
        {
            yield return new WaitForSeconds(attackRepeatTime);

            // Play attack animation
            animator.SetTrigger("Attack");

            // Detect player in range of attack
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attack1Point.position, attack1Range, playerLayerMask);

            // Damage them
            foreach (Collider2D player in hitPlayer)
            {
                player.transform.parent.GetComponent<Player>().TakeDamage(attack1Damage);
            }
        }
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
            player.transform.parent.GetComponent<Player>().TakeDamage(attack1Damage);
        }
    }
}
