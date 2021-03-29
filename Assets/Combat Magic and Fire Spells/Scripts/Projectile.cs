using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum TravelTrajectory
    {
        Horizontal,
        Parabolic
    }

    public enum CharacterFiringProjectile
    {
        Player,
        Enemy
    }

    // Private references
    private Rigidbody2D rb;

    // Public variables
    public float projectileSpeed;
    public TravelTrajectory travelTrajectory;
    public float parabolicAngle;
    public bool hasImpactEffect;
    public GameObject impactEffectPrefab;
    public int spellDamage;
    public CharacterFiringProjectile character;

    private Task destroyInstanceTask;

    // Start is called before the first frame update
    void Start()
    {
        // Prevent collision with combat block walls
        Physics2D.IgnoreLayerCollision(gameObject.layer, 15);

        rb = GetComponent<Rigidbody2D>();

        if (travelTrajectory == TravelTrajectory.Horizontal)
        {
            rb.gravityScale = 0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            rb.gravityScale = 1f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, parabolicAngle);
        }

        rb.velocity = transform.right * projectileSpeed;

        // Self destruct after delay
        destroyInstanceTask = new Task(DestroyInstanceAfterDelay());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (character == CharacterFiringProjectile.Player)
        {
            // Damage enemy or boss
            if (collision.CompareTag("Enemy"))
            {
                if (hasImpactEffect)
                {
                    SpawnImpactEffect();
                }
                else
                {
                    DestroyProjectileInstance();
                }

                collision.transform.parent.GetComponent<RangedEnemyCombat>().TakeDamage(transform, spellDamage);
            }
            else if (collision.CompareTag("Boss"))
            {
                if (hasImpactEffect)
                {
                    SpawnImpactEffect();
                }
                else
                {
                    DestroyProjectileInstance();
                }

                collision.transform.parent.GetComponent<BossCombat>().TakeDamage(spellDamage);
            }
        }
        else if (character == CharacterFiringProjectile.Enemy)
        {
            // Damage the player
            if (collision.CompareTag("Player"))
            {
                if (hasImpactEffect)
                {
                    SpawnImpactEffect();
                }
                else
                {
                    DestroyProjectileInstance();
                }

                PlayerCombat.instance.TakeDamage(transform, spellDamage);
            }
        }
    }

    private void SpawnImpactEffect()
    {
        // Spawn impact effect
        GameObject impact = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

        // Destroy impact effect object
        Destroy(impact, 1f);

        DestroyProjectileInstance();
    }

    private void DestroyProjectileInstance()
    {
        // Destroy this spell particle
        Destroy(gameObject);

        // Stop self destruct task
        destroyInstanceTask.Stop();
    }

    private IEnumerator DestroyInstanceAfterDelay()
    {
        yield return new WaitForSeconds(10f);

        if (gameObject != null)
            Destroy(gameObject);
    }
}
