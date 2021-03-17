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
        
        if (attackWeapon == PlayerCombat.WeaponTypes.Sword)
        {
            // TODO: add sword damage int when ready
        }

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponentInParent<EnemyCombat>().TakeDamage(transform.parent.transform, damageAmount);
            
            if (collision.GetComponentInParent<EnemyCombat>().IsDead)
                collision.enabled = false;
        }
    }
}
