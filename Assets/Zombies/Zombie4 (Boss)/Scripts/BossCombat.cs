using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BossCombat : MonoBehaviour
{
    public enum AttackTypes
    {
        Attack1,
        Attack2
    }

    // Public attack attributes
    [Header("Attack 1 Attributes")]
    public int attack1Damage;
    public float attack1RepeatTime;
    public Transform attack1_ParticleSpawnLocation;
    public GameObject attack1_ParticlePrefab;

    [Header("Attack 2 Attributes")]
    public int attack2Damage;
    public float attack2RepeatTime;
    public GameObject attack2SpawnLocations;
    public GameObject attack2_ParticlePrefab;

    [HideInInspector]
    public bool canAttack = false;

    // Private references
    private Animator animator;
    private BossAI bossAI;

    // Public references
    [Header("References")]
    public HealthBar healthBar;
    public Transform bloodParticlesStartPosition;
    public Light2D regenerationLight;
    public BoxCollider2D damageAreaCollider;

    // Private general variables
    private int currentHealth;
    private Task attackTask = null;

    // Public general variables
    [Header("Enemy Attributes")]
    public int maxHealth;
    public float destroyDelayAfterDeath = 3f;
    public float pushDistanceOnHit;
    public float damageMultiplier = 1f;

    // State bools
    [HideInInspector]
    public bool isBeingPushed = false;
    [HideInInspector]
    public bool Attack1Active = false;
    [HideInInspector]
    public bool Attack2Active = false;
    [HideInInspector]
    public bool firstRegenerationComplete = false;
    [HideInInspector]
    public bool secondRegenerationComplete = false;

    [HideInInspector]
    public AttackTypes currentAttackType = AttackTypes.Attack1;

    // Death Event
    public Action<BossCombat> OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        SetReferences();
        Initialize();
    }

    private void SetReferences()
    {
        bossAI = GetComponent<BossAI>();
        animator = GetComponent<Animator>();
    }

    private void Initialize()
    {
        currentHealth = maxHealth;
        healthBar.gameObject.SetActive(true);
        healthBar.SetMaxHealth(maxHealth);
        regenerationLight.intensity = 0f;
    }

    public void StopAttack()
    {
        if (canAttack == true)
        {
            canAttack = false;

            if (attackTask != null)
            {
                attackTask.Stop();
                attackTask = null;
            }
        }

        Attack1Active = false;
        Attack2Active = false;
    }

    public void InvokeAttack1()
    {
        canAttack = true;
        animator = GetComponent<Animator>();

        if (bossAI.state == BossAI.BossState.Attacking)
            return;

        if (bossAI.state == BossAI.BossState.Dead)
            return;

        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        attackTask = new Task(Attack1());
    }

    private IEnumerator Attack1()
    {
        while (canAttack)
        {
            if (bossAI.state == BossAI.BossState.Dead)
            {
                canAttack = false;
                Attack1Active = false;
                yield break;
            }

            bossAI.state = BossAI.BossState.Attacking;

            Attack1Active = true;

            // Play attack 1 sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.Zombie4Attack1);

            // Play attack 1 animation
            animator.SetTrigger("Attack1");

            // Spawn attack 1 particles
            new Task(SpawnAttack1Particles());

            yield return new WaitForSeconds(attack1RepeatTime);
        }
        Attack1Active = false;
    }

    private IEnumerator SpawnAttack1Particles()
    {
        // Wait for anim to reach particle spawn location
        yield return new WaitForSeconds(0.25f);

        Vector3 vectorToPlayer = PlayerCombat.instance.bloodParticlesStartPosition.position - attack1_ParticleSpawnLocation.position;
        Vector3 rotatedVectorToPlayer = Quaternion.Euler(0f, 0f, 90f) * vectorToPlayer;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToPlayer);

        attack1_ParticleSpawnLocation.localRotation = Quaternion.RotateTowards(attack1_ParticleSpawnLocation.localRotation, targetRotation, 100f);

        // Spawn particles
        Instantiate(attack1_ParticlePrefab, attack1_ParticleSpawnLocation.position, attack1_ParticleSpawnLocation.localRotation);

        yield return new WaitForSeconds(0.1f);

        Instantiate(attack1_ParticlePrefab, attack1_ParticleSpawnLocation.position, attack1_ParticleSpawnLocation.localRotation);

        yield return new WaitForSeconds(0.1f); 
        
        Instantiate(attack1_ParticlePrefab, attack1_ParticleSpawnLocation.position, attack1_ParticleSpawnLocation.localRotation);
    }

    public void InvokeAttack2()
    {
        canAttack = true;
        animator = GetComponent<Animator>();

        if (bossAI.state == BossAI.BossState.Attacking)
            return;

        if (bossAI.state == BossAI.BossState.Dead)
            return;

        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        attackTask = new Task(Attack2());
    }

    private IEnumerator Attack2()
    {
        while (canAttack)
        {
            if (bossAI.state == BossAI.BossState.Dead)
            {
                canAttack = false;
                Attack2Active = false;
                yield break;
            }

            bossAI.state = BossAI.BossState.Attacking;

            Attack2Active = true;

            // Play attack 2 sound
            AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.Zombie4Attack2);

            // Play attack 2 animation
            animator.SetTrigger("Attack2");

            // Spawn attack 2 particles
            new Task(SpawnAttack2Particles());

            yield return new WaitForSeconds(attack2RepeatTime);
        }
        Attack2Active = false;
    }

    private IEnumerator SpawnAttack2Particles()
    {
        // Wait for anim to reach particle spawn location
        yield return new WaitForSeconds(0.25f);

        Vector3 vectorToPlayer;
        Vector3 rotatedVectorToPlayer;
        Quaternion targetRotation;

        foreach (Transform child in attack2SpawnLocations.transform)
        {
            vectorToPlayer = PlayerCombat.instance.bloodParticlesStartPosition.position - child.position;
            rotatedVectorToPlayer = Quaternion.Euler(0f, 0f, 90f) * vectorToPlayer;
            targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToPlayer);
            child.localRotation = Quaternion.RotateTowards(child.localRotation, targetRotation, 100f);

            // Spawn particle
            Instantiate(attack2_ParticlePrefab, child.position, child.localRotation);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        Attack1Active = false;
        Attack2Active = false;

        if (bossAI.state == BossAI.BossState.Dead)
            return;

        if (bossAI.state == BossAI.BossState.KneelDown)
            return;

        // Play getting hit sound
        AudioManager.PlayOneShotSound(AudioManager.Sound.Zombie4GettingHit);

        // Create blood particles
        GameObject bloodParticles = Instantiate(GameAssets.instance.bloodParticles, bloodParticlesStartPosition);
        bloodParticles.transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        StartCoroutine(DestroyGameObjectAfterDelay(bloodParticles, 5f));

        // Play hurt animation
        animator.SetTrigger("Hurt");

        // Go into kneel down status if health less than 60 for the first time
        if (currentHealth > 60)
        {
            // Push boss in hit direction
            StartCoroutine(PushBossInHitDirection());

            damageMultiplier = 1f;

            // Reduce health
            currentHealth -= (int)Mathf.Floor(damageAmount * damageMultiplier);

            // Update Health Bar
            healthBar.SetHealth(currentHealth);
        }
        else if (currentHealth <= 60 && currentHealth > 30)
        {
            if (!firstRegenerationComplete)
            {
                // Set first regeneration bool to complete
                firstRegenerationComplete = true;

                // Set boss status
                bossAI.state = BossAI.BossState.KneelDown;

                // Disable damage area collider
                damageAreaCollider.enabled = false;

                // Stop current attack
                StopAttack();

                // Kneel down 
                animator.SetBool("KneelDown", true);

                // Regenerate health
                new Task(StartRegeneratingHealthAfterDelay(currentHealth, currentHealth + 20f, 1f, 4f));
            }
            else
            {
                // Push boss in hit direction
                StartCoroutine(PushBossInHitDirection());

                damageMultiplier = 1f;

                // Reduce health
                currentHealth -= (int)Mathf.Floor(damageAmount * damageMultiplier);

                // Update Health Bar
                healthBar.SetHealth(currentHealth);
            }
        }
        else if (currentHealth <= 30)
        {
            if (!secondRegenerationComplete)
            {
                // Set second regeneration bool to complete
                secondRegenerationComplete = true;

                // Set boss status
                bossAI.state = BossAI.BossState.KneelDown;

                // Disable damage area collider
                damageAreaCollider.enabled = false;

                // Stop current attack
                StopAttack();

                // Kneel down 
                animator.SetBool("KneelDown", true);

                // Regenerate health
                new Task(StartRegeneratingHealthAfterDelay(currentHealth, currentHealth + 10f, 1f, 4f));
            }
            else
            {
                // Push boss in hit direction
                StartCoroutine(PushBossInHitDirection());

                damageMultiplier = 1f;

                // Reduce health
                currentHealth -= (int)Mathf.Floor(damageAmount * damageMultiplier);

                // Update Health Bar
                healthBar.SetHealth(currentHealth);
            }
        }

        // Die if health is less than 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator StartRegeneratingHealthAfterDelay(float startHealth, float endHealth, float delay, float lerpTime = 4f)
    {
        bossAI.state = BossAI.BossState.KneelDown;

        yield return new WaitForSeconds(delay);

        AudioManager.PlaySoundOnceOnPersistentObject(AudioManager.Sound.BossHealthRecharge);

        new Task(FlashRegenerationLight());

        float _timeStartedLerping = Time.unscaledTime;
        float timeSinceStarted;
        float percentageComplete;

        while (true)
        {
            timeSinceStarted = Time.unscaledTime - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(startHealth, endHealth, percentageComplete);

            if (healthBar != null)
            {
                currentHealth = (int)Mathf.Floor(currentValue);

                // Update Health Bar
                healthBar.SetHealth(currentHealth);
            }

            if (percentageComplete >= 1)
            {
                // Go back to idle anim
                animator.SetBool("KneelDown", false);
                bossAI.state = BossAI.BossState.Chasing;

                // Enable damage area collider
                damageAreaCollider.enabled = true;

                // Switch attack type
                if (currentAttackType == AttackTypes.Attack1)
                {
                    currentAttackType = AttackTypes.Attack2;
                }
                else
                {
                    currentAttackType = AttackTypes.Attack1;
                }

                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator FlashRegenerationLight()
    {
        while (bossAI.state == BossAI.BossState.KneelDown)
        {
            bossAI.state = BossAI.BossState.KneelDown;
            regenerationLight.enabled = true;
            new Task(InterpolateLight2DIntensity(regenerationLight, 5.5f, 10.5f, 0f, 0.7f));

            yield return new WaitForSeconds(0.7f);

            new Task(InterpolateLight2DIntensity(regenerationLight, 10.5f, 5.5f, 0f, 0.7f));
            
            yield return new WaitForSeconds(0.7f);
        }

        regenerationLight.enabled = false;
        regenerationLight.intensity = 0f;
    }

    private IEnumerator InterpolateLight2DIntensity(Light2D light, float startIntensity, float endIntensity, float delay, float lerpTime = 4f)
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(startIntensity, endIntensity, percentageComplete);

            if (light != null)
            {
                light.intensity = currentValue;
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator PushBossInHitDirection()
    {
        // Get rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Set boss state to taking damage
        // bossAI.state = BossAI.BossState.TakingDamage;

        isBeingPushed = true;

        // Push boss to the left
        rb.velocity = new Vector2(-pushDistanceOnHit, rb.velocity.y);
        yield return new WaitForSeconds(0.7f);

        isBeingPushed = false;

        // Set boss state to chasing

        // bossAI.state = BossAI.BossState.Chasing;
    }

    public void Die()
    {
        // Set boss state to dead
        bossAI.state = BossAI.BossState.Dead;

        // Stop attack task
        if (attackTask != null)
        {
            attackTask.Stop();
            attackTask = null;
        }

        // Play death sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.Zombie4Death);

        // Set die animation parameter
        animator.SetBool("IsDead", true);

        // Stop pathfinding
        bossAI.followPath = false;
        bossAI.StopMovement();

        // Invoke Death Event
        OnDeath?.Invoke(this);

        // Set the boss to layer 13 (DeadZombie) to prevent collision with player
        gameObject.layer = 13;

        // Disable Health Bar
        healthBar.gameObject.SetActive(false);

        // Notification to game ending script
        GameObject.Find("BehaviourAfterBossDefeated").GetComponent<BehaviourAfterBossDefeated>().StartGameEndBehaviour();
    }

    private IEnumerator DestroyGameObjectAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
