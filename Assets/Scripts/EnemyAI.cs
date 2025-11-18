using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- Public References & Health Settings ---
    [Tooltip("The player's Transform component.")]
    public Transform player;

    [Header("Health")]
    [Tooltip("Current health of the enemy.")]
    [SerializeField] private int currentHealth = 50;

    [Header("Movement & Speed")]
    [Tooltip("Speed of the enemy capsule.")]
    [SerializeField] private float moveSpeed = 2.0f;
    [Tooltip("Time the enemy waits before choosing a new direction.")]
    [SerializeField] private float waitTime = 2.0f;

    [Header("Shooting & Range")]
    [Tooltip("Prefab of the projectile the enemy shoots.")]
    public GameObject bulletPrefab;
    [Tooltip("Point where the bullet is instantiated (child of the Capsule).")]
    public Transform shotSpawn;
    [Tooltip("Distance at which the enemy starts shooting.")]
    [SerializeField] private float attackRange = 10.0f;
    [Tooltip("Time between shots.")]
    [SerializeField] private float fireRate = 1.0f;

    // --- Private Variables ---
    private CharacterController controller;
    private Vector3 moveDirection;
    private float nextMoveTime;
    private float nextFireTime;
    private float gravity = -30f;
    private Vector3 velocity; // For gravity

    void Awake()
    {
        // Get the CharacterController component
        controller = GetComponent<CharacterController>();

        // Try to automatically find the player if not manually assigned (requires "Player" tag)
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        nextMoveTime = Time.time;
        nextFireTime = Time.time;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (player == null) return;

        // Apply gravity and general movement
        ApplyGravity();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            HandleShooting();
        }
        else
        {
            HandleMovement();
        }
    }

    // --- Core Methods ---

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            velocity.y = -2f; // Keep grounded firmly
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // 1. Check if it's time to choose a new direction
        if (Time.time > nextMoveTime)
        {
            ChooseNewDirection();
        }

        // 2. Obstacle Avoidance: check for walls or falling off ledges
        if (IsWallInFront() || IsNearEdge())
        {
            ChooseNewDirection();
        }

        // 3. Execute Movement (using CharacterController.Move for collision)
        Vector3 horizontalMove = moveDirection * moveSpeed * Time.deltaTime;
        controller.Move(horizontalMove);

        // Rotate the capsule to face the movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void ChooseNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        // Calculate new direction vector
        moveDirection = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        moveDirection.Normalize();

        nextMoveTime = Time.time + waitTime;
    }

    private bool IsWallInFront()
    {
        // Check for walls 1.5 units in front
        return Physics.Raycast(transform.position, transform.forward, 1.5f);
    }

    private bool IsNearEdge()
    {
        // Check for ground 1 unit ahead and 2 units down
        Vector3 checkPos = transform.position + transform.forward * 1.0f;
        return !Physics.Raycast(checkPos, Vector3.down, 2.0f);
    }

    private void HandleShooting()
    {
        // 1. Look at the player
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        // 2. Fire if cooldown is ready
        if (Time.time > nextFireTime && shotSpawn != null && bulletPrefab != null)
        {
            nextFireTime = Time.time + fireRate;

            // Instantiate and launch the bullet
            GameObject bullet = Instantiate(bulletPrefab, shotSpawn.position, shotSpawn.rotation);

            // NOTE: You need a script on the bullet prefab to handle its forward movement!
            // Example if bullet has a Rigidbody:
            // float bulletSpeed = 20f;
            // bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        }
    }

    // --- Health System ---

    // Call this function from your player's bullet script when the enemy is hit
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Optional: Add visual feedback here (e.g., flash red)
    }

    private void Die()
    {
        // Optional: Play explosion effect, drop loot, etc.
        Debug.Log(gameObject.name + " destroyed!");
        Destroy(gameObject);
    }
}