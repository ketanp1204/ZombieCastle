using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EnemyCombat))]
public class EnemyAI : MonoBehaviour
{ 
    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        TakingDamage,
        Dead
    }

    // Private Cached References
    private Transform target;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyCombat enemyCombat;

    // Public Variables
    public float attackStartDistance;
    [HideInInspector]
    public bool isAttacking;
    private float distanceToPlayer;
    [HideInInspector]
    public bool facingRight = true;

    [Header("Pathfinding")]
    public float pathUpdateSeconds;
    [HideInInspector]
    public bool followPath;
    private Path path;
    private int currentWaypoint = 0;
    // private bool reachedEndOfPath = false;

    [Header("Physics")]
    public float speed = 60f;
    public float nextWaypointDistance = 3f;
    public float moveForceMultiplier = 19f;

    [HideInInspector]
    public EnemyState enemyState;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemyCombat = GetComponent<EnemyCombat>();
        animator = GetComponent<Animator>();
        followPath = false;
        isAttacking = false;

        enemyState = EnemyState.Idle;

        // Ignore enemy collisions with other enemies   TODO: remove if it looks bad
        // Physics2D.IgnoreLayerCollision(gameObject.layer, 11);

        // InvokeRepeating("UpdatePath", 3f, pathUpdateSeconds);
    }

    public void StartChasingPlayer()
    {
        enemyState = EnemyState.Chasing;
        followPath = true;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    void UpdatePath()
    {
        if (followPath && !enemyCombat.IsDead && !PlayerStats.IsDead)
        {
            target = Player.instance.pathfindingTarget;
            if (seeker.IsDone())
            {
                enemyCombat.StopAttack();
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }
    }

    void FixedUpdate()
    {
        if(followPath && !enemyCombat.IsDead && !PlayerStats.IsDead && enemyState != EnemyState.TakingDamage)
            FollowPath();

        if (PlayerStats.IsDead)
        {
            enemyCombat.StopAttack();
        }
    }

    private void FollowPath()
    {
        if (path == null)
            return;

        Vector2 direction;
        Vector2 movement;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count || distanceToPlayer < attackStartDistance)
        {
            followPath = false;
            isAttacking = true;
            movement = Vector2.zero;

            enemyState = EnemyState.Attacking;
            enemyCombat.InvokeAttack();
            StartCoroutine(WaitForPlayerToMoveAway());
        } 
        else
        {
            direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

            //movement.x = direction.x * moveForceMultiplier;
            movement.x = direction.x;
            movement.y = 0f;

            // Move the enemy
            if (!enemyCombat.takingDamage)
            {
                rb.velocity = new Vector2(movement.x * speed * Time.deltaTime, rb.velocity.y);
            }

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            // Update facing direction of enemy
            if (movement.x >= 0.01f)
            {
                UpdateFaceDirection(true);
                facingRight = true;
            }
            else if (movement.x <= -0.01f)
            {
                UpdateFaceDirection(false);
                facingRight = false;
            }
        }

        // Set movement animation parameters
        SetMovementAnimationParameters(movement);
    }

    public void UpdateFaceDirection(bool isFacingRight)
    {
        if (isFacingRight)
        {
            transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void SetMovementAnimationParameters(Vector2 movement)
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Magnitude", movement.magnitude);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    private IEnumerator WaitForPlayerToMoveAway()
    {
        target = Player.instance.pathfindingTarget;
        distanceToPlayer = Vector2.Distance(transform.position, target.position);

        while (distanceToPlayer < attackStartDistance)
        {
            yield return new WaitForSeconds(pathUpdateSeconds);
            distanceToPlayer = Vector2.Distance(transform.position, target.position);
        }

        followPath = true;
    }
}
