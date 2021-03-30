using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTheEnemy : MonoBehaviour
{
    public PlayerCombat.WeaponTypes attackWeapon;
    private int damageAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackWeapon == PlayerCombat.WeaponTypes.Knife)
        {
            damageAmount = PlayerCombat.instance.knifeDamage;
        }

        if (attackWeapon == PlayerCombat.WeaponTypes.Axe)
        {
            damageAmount = PlayerCombat.instance.axeDamage;
        }

        if (collision.CompareTag("Enemy"))
        {
            if (attackWeapon == PlayerCombat.WeaponTypes.Sword)
            {
                collision.GetComponentInParent<RangedEnemyCombat>().TakeDamage(transform.parent.transform, damageAmount);

                if (collision.GetComponentInParent<RangedEnemyAI>().enemyState == RangedEnemyAI.EnemyState.Dead)
                    collision.enabled = false;
            }
            else
            {
                collision.GetComponentInParent<EnemyCombat>().TakeDamage(transform.parent.transform, damageAmount);

                if (collision.GetComponentInParent<EnemyAI>().enemyState == EnemyAI.EnemyState.Dead)
                    collision.enabled = false;
            }
        }
    }
}
