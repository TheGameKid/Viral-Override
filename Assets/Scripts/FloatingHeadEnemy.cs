using UnityEngine;

public class FloatingHeadEnemy : MonoBehaviour
{
    // --- Public Variables (Set in Inspector) ---
    [Header("Health")]
    public float maxHealth = 1000f; // The total health of the enemy

    [Header("Targeting and Firing")]
    public Transform player;
    public GameObject bulletPrefab;
    public float fireRate = 2f;
    public float bulletSpeed = 10f;
    public Transform firePoint;

    [Header("Movement and Appearance")]
    public float rotationSpeed = 5f;
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 1f;

    [Header("Horizontal Movement")]
    public float moveSpeed = 3f;
    public float patrolRadius = 10f;
    public float moveChangeTime = 4f;

    [Header("Audio")] // 🔊 NEW SECTION FOR AUDIO
    [Tooltip("The continuous sound clip for movement/hovering.")]
    public AudioClip movementSoundClip;
    [Tooltip("The AudioSource component used for movement sound.")]
    public AudioSource movementAudioSource; // Reference to the AudioSource component

    // --- Private Variables ---
    public float currentHealth; // Tracks the current health
    private float nextFireTime;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float nextMoveChangeTime;

    public Game game;

    void Start()
    {
        // 💡 NEW: Initialize current health
        currentHealth = maxHealth;

        startPosition = transform.position;
        nextFireTime = Time.time;
        nextMoveChangeTime = Time.time;
        ChooseNewTargetPosition();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Please assign the 'player' transform or tag the player object as 'Player'.");
                enabled = false;
            }
        }

        // 💡 NEW: Start playing the movement sound when the enemy activates
        if (movementAudioSource != null && movementSoundClip != null)
        {
            movementAudioSource.clip = movementSoundClip;
            movementAudioSource.loop = true; // Set to loop since the enemy is always moving/hovering
            movementAudioSource.Play();
        }
    }

    void Update()
    {
        if (player == null) return;

        // Movement and Shooting logic (remains the same)
        ApplyHorizontalMovement();
        ApplyHoverMovement();
        RotateToFacePlayer();
        CheckAndFire();

        game.BossHPText.text = "HP: " + currentHealth + "/" + maxHealth;
    }

    // --- New Health and Damage Methods ---

    /**
     * Public method to call when the enemy is hit by a projectile.
     */
    public void TakeDamage(float damageAmount)
    {
        // Subtract damage from current health
        currentHealth -= damageAmount;

        Debug.Log($"Head took {damageAmount} damage. Current Health: {currentHealth}");

        // Check if health has dropped to zero or below
        if (currentHealth <= 0)
        {
            game.BossHPText.text = "HP: " + currentHealth + "/" + maxHealth;
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(50);
            Destroy(other.gameObject);
        }
    }

    /**
     * Handles the enemy's defeat.
     */
    private void Die()
    {
        Debug.Log("Floating Head Destroyed!");

        // 💡 NEW: Stop the movement sound when the enemy is defeated
        if (movementAudioSource != null)
        {
            movementAudioSource.Stop();
        }

        // Finally, destroy the enemy GameObject
        this.gameObject.SetActive(false);
        // Note: You should generally call Destroy(gameObject) instead of SetActive(false)
        // unless you need the object for post-death effects.
    }

    // --- Core Logic Methods (Movement and Shooting) ---

    private void RotateToFacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void ApplyHoverMovement()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void ApplyHorizontalMovement()
    {
        if (Time.time > nextMoveChangeTime || Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            ChooseNewTargetPosition();
        }

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        Vector3 horizontalMove = moveDirection * moveSpeed * Time.deltaTime;
        horizontalMove.y = 0;
        transform.position += horizontalMove;
    }

    private void ChooseNewTargetPosition()
    {
        nextMoveChangeTime = Time.time + moveChangeTime;

        Vector2 randomCircle = Random.insideUnitCircle.normalized * patrolRadius;

        Vector3 currentHorizontalPosition = transform.position;
        currentHorizontalPosition.y = startPosition.y;

        Vector3 newTarget = currentHorizontalPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Optional: Clamp the new target to prevent endless drifting
        newTarget.x = Mathf.Clamp(newTarget.x, startPosition.x - patrolRadius, startPosition.x + patrolRadius);
        newTarget.z = Mathf.Clamp(newTarget.z, startPosition.z - patrolRadius, startPosition.z + patrolRadius);

        targetPosition = newTarget;
    }

    private void CheckAndFire()
    {
        if (Time.time > nextFireTime)
        {
            ShootBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}