using UnityEngine;
using System.Collections;

public class GrandVisageBoss : MonoBehaviour
{
    // --- Public Settings ---

    [Header("Boss Stats")]
    public float maxHealth = 1000f;
    private float currentHealth;
    public float moveSpeed = 4f;
    public Transform playerTarget;
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;

    [Header("Fixed Y Position")]
    // Set this value in the Inspector to the desired height
    public float fixedBossHeight = 15f;

    // Rotation Settings are REMOVED

    [Header("Attack Timers")]
    public float phase1AttackDelay = 3f;
    public float phase2AttackDelay = 2.5f;

    // --- Private Variables ---
    private int currentPhase = 1;
    private bool isAttacking = false;

    // --- References ---
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;

        // Find the player object by tag
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        // Set the boss to the fixed starting height immediately
        transform.position = new Vector3(transform.position.x, fixedBossHeight, transform.position.z);

        // Start the main attack loop
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        // 1. Player-Seeking Movement with Y-Lock
        HandleMovement();

        // 2. Phase Checking
        HandlePhaseTransition();
    }

    // ====================================================================
    // --- MOVEMENT LOGIC (Y-Axis Locked, NO ROTATION) ---
    // ====================================================================

    private void HandleMovement()
    {
        if (playerTarget == null)
        {
            return;
        }

        Vector3 playerPos = playerTarget.position;

        // Target X and Z is the player's position, Y is the fixed height
        Vector3 chaseTarget = new Vector3(playerPos.x, fixedBossHeight, playerPos.z);

        // Move smoothly towards the horizontal location of the player at the fixed height
        transform.position = Vector3.MoveTowards(
            transform.position,
            chaseTarget,
            moveSpeed * Time.deltaTime
        );

        // **ROTATION CODE IS REMOVED**
    }

    private void HandlePhaseTransition()
    {
        float healthPercent = currentHealth / maxHealth;

        if (healthPercent <= 0.33f && currentPhase < 3)
        {
            currentPhase = 3;
            Debug.Log("Grand Visage: Entering Phase 3 - THE RECKONING!");
        }
        else if (healthPercent <= 0.66f && currentPhase < 2)
        {
            currentPhase = 2;
            Debug.Log("Grand Visage: Entering Phase 2 - THE SEARING GAZE!");
        }
    }

    // ====================================================================
    // --- ATTACK LOGIC (Unchanged) ---
    // ====================================================================

    IEnumerator AttackRoutine()
    {
        while (currentHealth > 0)
        {
            float delay = (currentPhase == 1) ? phase1AttackDelay : phase2AttackDelay;

            yield return new WaitForSeconds(delay);

            if (isAttacking) continue;

            int attackIndex = Random.Range(1, 4);

            switch (currentPhase)
            {
                case 1:
                    if (attackIndex == 1) StartCoroutine(Attack1_EyeBeams());
                    else if (attackIndex == 2) StartCoroutine(Attack2_GroundingStomp());
                    else StartCoroutine(Attack1_EyeBeams());
                    break;

                case 2:
                    if (attackIndex == 1) StartCoroutine(Attack3_BurstingMouthCannon(5));
                    else if (attackIndex == 2) StartCoroutine(Attack2_GroundingStomp());
                    else StartCoroutine(Attack3_BurstingMouthCannon(7));
                    break;

                case 3:
                    if (attackIndex == 1) StartCoroutine(Attack5_AnnihilationRay());
                    else if (attackIndex == 2) StartCoroutine(Attack3_BurstingMouthCannon(10));
                    else StartCoroutine(Attack5_AnnihilationRay());
                    break;
            }
        }
    }

    IEnumerator Attack1_EyeBeams()
    {
        isAttacking = true;
        Debug.Log("Visage Attack: Eye Beams");

        for (int i = 0; i < 4; i++)
        {
            if (playerTarget != null)
            {
                ShootProjectileAt(playerTarget.position);
            }
            yield return new WaitForSeconds(0.2f);
        }

        isAttacking = false;
    }

    IEnumerator Attack2_GroundingStomp()
    {
        isAttacking = true;
        Debug.Log("Visage Attack: Grounding Stomp");

        yield return new WaitForSeconds(1.0f);

        if (playerTarget != null)
        {
            ShootProjectileAt(playerTarget.position, 30f);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    IEnumerator Attack3_BurstingMouthCannon(int count)
    {
        isAttacking = true;
        Debug.Log("Visage Attack: Bursting Mouth Cannon x" + count);

        yield return new WaitForSeconds(1.5f);

        Vector3 targetPos = (playerTarget != null) ? playerTarget.position : Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            Vector3 spread = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            ShootProjectileAt(targetPos + spread);
            yield return new WaitForSeconds(0.1f);
        }

        isAttacking = false;
    }

    IEnumerator Attack5_AnnihilationRay()
    {
        isAttacking = true;
        Debug.Log("Visage Attack: Annihilation Ray (8s Active)");

        yield return new WaitForSeconds(4.0f);

        for (int i = 0; i < 8; i++)
        {
            Vector3 centerTarget = new Vector3(0, 0, 0);
            ShootProjectileAt(centerTarget, 20f);
            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
    }

    // ====================================================================
    // --- UTILITY & DAMAGE ---
    // ====================================================================

    void ShootProjectileAt(Vector3 target, float speedOverride = -1f)
    {
        if (projectilePrefab == null) return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (target - transform.position).normalized;
            float speed = (speedOverride > 0) ? speedOverride : projectileSpeed;

            rb.linearVelocity = direction * speed;

            Destroy(projectile, 5f);
        }
        else
        {
            Debug.LogError("Projectile prefab needs a Rigidbody component.");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss took " + damage + " damage. Remaining Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StopAllCoroutines();
        Debug.Log("Grand Visage has been defeated!");

        Destroy(gameObject);
    }
}