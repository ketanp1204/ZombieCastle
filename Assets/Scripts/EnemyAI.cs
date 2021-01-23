using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    private Transform target;
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

    // Zombie sprite object
    private GameObject zombieGFX;

    // Cached References
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyCombat enemyCombat;
    private Enemy enemy;
    private SceneType sceneTypeReference;

    // Variables
    private int sceneType;
    public float attackStartDistance = 3f;

    // Start is called before the first frame update
    void Start()
    {
        zombieGFX = transform.GetChild(0).gameObject;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().currentPathfindingTarget.transform;
        enemyCombat = GetComponent<EnemyCombat>();
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
        sceneTypeReference = FindObjectOfType<SceneType>();
        followPath = true;

        if (sceneTypeReference.type == SceneType.SceneTypes.S_TwoD)
        {
            sceneType = 1;
        }
        else if (sceneTypeReference.type == SceneType.SceneTypes.S_TwoPointFiveD)
        {
            sceneType = 2;
        }

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    void UpdatePath()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (seeker.IsDone() && followPath && distanceToPlayer > attackStartDistance)
        {
            enemyCombat.StopAttack();
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        } 
    }

    void FixedUpdate()
    {
        if(followPath && !enemy.IsDead)
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
            force = Vector2.zero;
            enemyCombat.InvokeAttack();
        } 
        else
        {
            direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            
            // Change movement applied on enemy based on scene type 
            if (sceneType == 1)
            {
                force = new Vector2(direction.x * moveForceMultiplier, direction.y) * speed * Time.fixedDeltaTime;
            }
            else
            {
                force = direction * speed * Time.fixedDeltaTime;
            }

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
        if(sceneType == 2)
        {
            animator.SetFloat("Vertical", movement.y);
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

    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            followPath = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            followPath = true;
        }
    }
}
