using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    private Transform target;
    public float pathUpdateSeconds;
    public bool followPath;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    [Header("Physics")]
    public float speed = 60f;
    public float nextWaypointDistance = 3f;

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

    // Start is called before the first frame update
    void Start()
    {
        zombieGFX = transform.GetChild(0).gameObject;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<Player>().transform;
        enemyCombat = GetComponent<EnemyCombat>();
        enemy = GetComponent<Enemy>();
        animator = zombieGFX.GetComponent<Animator>();
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
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
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

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            force = Vector2.zero;
            enemyCombat.attack1Trigger = true;
        } 
        else
        {
            direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            force = direction * speed * Time.fixedDeltaTime;

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
                zombieGFX.transform.localScale = new Vector3(1f * Mathf.Abs(zombieGFX.transform.localScale.x), zombieGFX.transform.localScale.y, zombieGFX.transform.localScale.z);
            }
            else if (force.x <= -0.01f)
            {
                zombieGFX.transform.localScale = new Vector3(-1f * Mathf.Abs(zombieGFX.transform.localScale.x), zombieGFX.transform.localScale.y, zombieGFX.transform.localScale.z);
            }
        }

        // Set movement animation parameters
        SetMovementAnimationParameters(force);
    }

    void SetMovementAnimationParameters(Vector2 movement)
    {
        if (sceneType == 1)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Magnitude", movement.magnitude);
        }
        else if(sceneType == 2)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Magnitude", movement.magnitude);
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
