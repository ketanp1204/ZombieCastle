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

    // Cached References
    private Transform target;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyCombat enemyCombat;

    // Variables
    public float attackStartDistance;
    [HideInInspector]
    public bool isAttacking;
    private float distanceToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemyCombat = GetComponent<EnemyCombat>();
        animator = GetComponent<Animator>();
        followPath = true;
        isAttacking = false;

        InvokeRepeating("UpdatePath", 1f, pathUpdateSeconds);
    }

    void UpdatePath()
    {
        if (followPath && !EnemyCombat.instance.IsDead)
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
        if(followPath && !EnemyCombat.instance.IsDead)
            FollowPath();
    }

    private void FollowPath()
    {
        if (path == null)
            return;

        Vector2 direction;
        Vector2 force;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count || distanceToPlayer < attackStartDistance)
        {
            followPath = false;
            isAttacking = true;
            force = Vector2.zero;
            enemyCombat.InvokeAttack();
            StartCoroutine(WaitForPlayerToMoveAway());
        } 
        else
        {
            direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            
            force = new Vector2(direction.x * moveForceMultiplier, rb.velocity.y) * speed * Time.fixedDeltaTime;

            // Move the enemy
            rb.AddForce(force);

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            // Update facing direction of enemy
            if (force.x >= 0.01f)
            {
                transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (force.x <= -0.01f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        // Set movement animation parameters
        SetMovementAnimationParameters(force);
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
