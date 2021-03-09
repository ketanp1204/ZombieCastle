using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageThePlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCombat.instance.TakeDamage(transform.parent.transform, transform.GetComponentInParent<EnemyCombat>().attack1Damage);
            // PlayerCombat.instance.TakeDamage(transform.parent.transform, EnemyCombat.instance.attack1Damage);
        }
    }
}
