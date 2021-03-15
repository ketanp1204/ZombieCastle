using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageThePlayer : MonoBehaviour
{

    public void StopAttackHitboxAnimation()
    {
        GetComponent<Animator>().SetBool("IsAttacking", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCombat.instance.TakeDamage(transform.parent.transform, transform.GetComponentInParent<EnemyCombat>().attack1Damage);

            if (PlayerStats.IsDead)
                collision.enabled = false;
        }
    }
}
