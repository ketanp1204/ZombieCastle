using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(BossCombat))]
public class BossAI : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Chasing,
        Attacking,
        KneelDown,
        TakingDamage,
        Dead
    }

    // Private References
    private Transform target;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;
    private BossCombat bossCombat;

    // Public Variables
    public float attackStartDistance;
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

    public BossState state;

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
        animator = GetComponent<Animator>();
        bossCombat = GetComponent<BossCombat>();
    }

    private void Initialize()
    {
        followPath = false;
        state = BossState.Idle;
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
        while (state != BossState.Dead)
        {
            yield return new WaitForSeconds(7f);

            if (state != BossState.Dead)
            {
                // Play roar sound
                AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.Zombie4Roar);
            }
        }
    }

    private IEnumerator UpdatePathfindingPath()
    {
        while (true)
        {
            if (!followPath)
                break;

            if (state == BossState.Dead)
                break;

            if (state == BossState.KneelDown)
                break;

            if (state == BossState.TakingDamage)
                break;

            if (PlayerStats.IsDead)
                break;


            // Get player pathfinding target transform
            target = Player.instance.pathfindingTarget;

            if (seeker.IsDone())
            {
                bossCombat.StopAttack();
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
            bossCombat.StopAttack();
        }
    }

    private void FollowPath()
    {
        if (path == null)
            return;

        if (!followPath)
            return;

        if (state == BossState.Dead)
            return;

        if (state == BossState.KneelDown)
            return;

        if (state == BossState.TakingDamage)
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

            StartCoroutine(WaitForPlayerToMoveAway());
        }
        else // Chasing
        {
            bossCombat.StopAttack();

            if (state != BossState.KneelDown)
            {
                // Debug.Log("changing here");
                state = BossState.Chasing;

                animator.SetBool("ChasingPlayer", true);

                direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

                movement.x = direction.x;
                movement.y = 0f;

                // Move the enemy
                if (!bossCombat.isBeingPushed)
                {
                    rb.velocity = new Vector2(movement.x * speed * Time.deltaTime, rb.velocity.y);
                }

                // Next Waypoint
                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }
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
            if (state != BossState.Dead && state != BossState.KneelDown)
            {
                if (bossCombat.currentAttackType == BossCombat.AttackTypes.Attack1)
                {
                    if (!bossCombat.Attack1Active)
                    {
                        bossCombat.InvokeAttack1();
                    }

                    yield return new WaitForSeconds(bossCombat.attack1RepeatTime + 0.1f);
                }
                else
                {
                    if (!bossCombat.Attack2Active)
                    {
                        bossCombat.InvokeAttack2();
                    }

                    yield return new WaitForSeconds(bossCombat.attack2RepeatTime + 0.1f);
                }
            }
            else
            {
                yield return new WaitForSeconds(pathUpdateSeconds);
            }
            
            distanceToPlayer = Vector2.Distance(transform.position, target.position);
        }

        StartChasingPlayer();
    }
}
