using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack 1 Attributes")]
    [HideInInspector]
    public bool attack1Trigger;
    public Transform attack1Point;      // Location of point which registers attack
    public float attack1Range;          // Range at which attack is enabled
    public int attack1Damage = 20;      // Damage caused to enemy

    public LayerMask playerLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        if (attack1Trigger)
            Attack1();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Attack1()
    {
        // Detect player in range of attack
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attack1Point.position, attack1Range, playerLayerMask);

        // Damage them
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<Player>().TakeDamage(attack1Damage);
        }
    }
}
