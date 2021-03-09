using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTheEnemy : MonoBehaviour
{
    public enum AttackWeapon
    {
        Axe,
        Knife
    }

    public AttackWeapon attackWeapon;
    private int damageAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackWeapon == AttackWeapon.Axe)
        {
            damageAmount = PlayerCombat.instance.axeDamage;
        }
        if (attackWeapon == AttackWeapon.Knife)
        {
            damageAmount = PlayerCombat.instance.knifeDamage;
        }

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponentInParent<EnemyCombat>().TakeDamage(transform.parent.transform, damageAmount);
            // EnemyCombat.instance.TakeDamage(transform.parent.transform, damageAmount);
        }
    }
}
