﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(RangedEnemyCombat))]
public class RangedEnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        TakingDamage,
        Dead
    }

    // Private References
    private Transform target;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;
    private RangedEnemyCombat rangedEnemyCombat;

    // Public Variables
    public float attackStartDistance;
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool combatStarted = false;

    [Header("Pathfinding")]
    public float pathUpdateSeconds;
    [HideInInspector]
    public bool followPath;

    [Header("Physics")]
    public float speed = 60f;
    public float nextWaypointDistance = 3f;
    public float moveForceMultiplier = 2.5f;

    public EnemyState enemyState;

    // Private variables
    private float distanceToPlayer;
    private Path path;
    private int currentWaypoint = 0;
    private Task pathUpdateTask = null;


    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        rangedEnemyCombat = GetComponent<RangedEnemyCombat>();
        animator = GetComponent<Animator>();
    }

    private void Initialize()
    {
        followPath = false;
        enemyState = EnemyState.Idle;
    }

    public void StartChasingPlayer()
    {
        if (!combatStarted)
        {
            combatStarted = true;
            new Task(PlayRoarSoundInDelays());
        }
        
        if (pathUpdateTask != null)
        {
            pathUpdateTask.Stop();
            pathUpdateTask = null;
        }

        followPath = true;

        pathUpdateTask = new Task(UpdatePathfindingPath());
    }

    private IEnumerator PlayRoarSoundInDelays()
    {
        while (enemyState != EnemyState.Dead)
        {
            yield return new WaitForSeconds(7f);

            if (enemyState != EnemyState.Dead)
            {
                // Play roar sound
                AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.Zombie3Roar);
            }
        }
    }

    private IEnumerator UpdatePathfindingPath()
    {
        while (true)
        {
            if (!followPath)
                break;

            if (enemyState == EnemyState.Dead)
                break;

            if (enemyState == EnemyState.TakingDamage)
                break;

            if (PlayerStats.IsDead)
                break;

            // Get player pathfinding target transform
            target = Player.instance.pathfindingTarget;

            if (seeker.IsDone())
            {
                rangedEnemyCombat.StopAttack();
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }

            yield return new WaitForSeconds(pathUpdateSeconds);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        FollowPath();

        if (PlayerStats.IsDead)
        {
            rangedEnemyCombat.StopAttack();
        }
    }

    private void FollowPath()
    {
        if (path == null)
        {
            return;
        }

        if (!followPath)
            return;

        if (enemyState == EnemyState.Dead)
            return;

        if (enemyState == EnemyState.TakingDamage)
            return;

        if (PlayerStats.IsDead)
            return;

        Vector2 direction;
        Vector2 movement; 

        distanceToPlayer = Vector2.Distance(transform.position, target.position);

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count || distanceToPlayer < attackStartDistance)
        {
            followPath = false;
            movement = Vector2.zero;

            animator.SetBool("ChasingPlayer", false);

            if (!rangedEnemyCombat.isAttacking)
            {
                rangedEnemyCombat.InvokeAttack();
            }

            StartCoroutine(WaitForPlayerToMoveAway());
        }
        else // Chasing
        {
            rangedEnemyCombat.StopAttack();

            animator.SetBool("ChasingPlayer", true);

            direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

            movement.x = direction.x;
            movement.y = 0f;

            // Move the enemy
            if (enemyState != EnemyState.TakingDamage)
            {
                rb.velocity = new Vector2(movement.x * speed * Time.deltaTime, rb.velocity.y);
            }

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            UpdateFaceDirection();
        }
    }

    public void UpdateFaceDirection()
    {
        if (target != null)
        {
            if ((target.position.x > transform.position.x) && !facingRight)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if ((target.position.x < transform.position.x) && facingRight)
            {
                transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
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
            if (!rangedEnemyCombat.isAttacking)
            {
                rangedEnemyCombat.InvokeAttack();
            }
            yield return new WaitForSeconds(rangedEnemyCombat.magicAttackRepeatTime + 0.1f);
            distanceToPlayer = Vector2.Distance(transform.position, target.position);
        }

        StartChasingPlayer();
    }
}
