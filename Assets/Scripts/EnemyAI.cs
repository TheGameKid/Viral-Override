using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- Public References & Settings ---

    // The player's transform to target 🎯
    [Tooltip("The player's Transform component.")]
    public Transform player;

    // Prefab of the bullet the enemy shoots
    [Tooltip("Prefab of the projectile the enemy shoots.")]
    public GameObject bulletPrefab;

    // Point where bullets are instantiated (child of the Capsule)
    [Tooltip("Point where the bullet is instantiated.")]
    public Transform shotSpawn;

    [Header("Movement & Speed")]
    [Tooltip("Speed of the enemy capsule.")]
    [SerializeField] private float moveSpeed = 2.0f;
    [Tooltip("Time the enemy waits before choosing a new direction.")]
    [SerializeField] private float waitTime = 2.0f;

    [Header("Shooting & Range")]
    [Tooltip("Distance at which the enemy starts and stops shooting.")]
    [SerializeField] private float attackRange = 10.0f;
    [Tooltip("Time between shots.")]
    [SerializeField] private float fireRate = 1.0f;

    // --- Private Variables ---
    private CharacterController controller;
    private Vector3 moveDirection;
    private float nextMoveTime;
    private float nextFireTime;
    private float gravity = -20f;
    private Vector3 velocity; // For gravity

    void Awake()
    {
        // Get the required components
        controller = GetComponent<CharacterController>();

        // Find the player if not manually assigned
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
        if (player == null) return;

        // Apply gravity to keep the enemy grounded
        ApplyGravity();

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // State: Attacking / Shooting
            HandleShooting();
        }
        else
        {
            // State: Patrolling / Walking
            HandleMovement();
        }
    }

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

    // --- Movement Logic ---
    private void HandleMovement()
    {
        // 1. Choose new direction when time is up
        if (Time.time > nextMoveTime)
        {
            ChooseNewDirection();
        }

        // 2. Check for Obstacles or Edges before moving
        if (IsWallInFront() || IsNearEdge())
        {
            // If blocked or near edge, choose new direction immediately
            ChooseNewDirection();
        }

        // 3. Execute Movement
        // Move only horizontally by projecting the direction onto the horizontal plane
        Vector3 horizontalMove = moveDirection * moveSpeed * Time.deltaTime;
        controller.Move(horizontalMove);

        // Rotate the capsule to face the direction of movement
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void ChooseNewDirection()
    {
        // Choose a random direction on the horizontal plane (X and Z)
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

        // Normalize the vector just in case
        moveDirection.Normalize();

        // Set the next time to change direction
        nextMoveTime = Time.time + waitTime;
    }

    // --- Obstacle Avoidance Checks ---

    private bool IsWallInFront()
    {
        // Raycast forward to check for a wall (Obstacle Avoidance)
        // Adjust the ray distance (e.g., 1.5f) to be slightly more than the capsule's radius.
        // It uses the enemy's current forward direction, which is updated in HandleMovement.
        return Physics.Raycast(transform.position, transform.forward, 1.5f);
    }

    private bool IsNearEdge()
    {
        // Raycast down slightly ahead of the current position to check for ground (Edge Avoidance)
        // Cast from a point ahead (Enemy position + forward direction * 1.0f)
        Vector3 checkPos = transform.position + transform.forward * 1.0f;

        // Raycast downwards (e.g., 2.0f distance)
        // If the ray does NOT hit the ground, the capsule is near an edge.
        return !Physics.Raycast(checkPos, Vector3.down, 2.0f);
    }

    // --- Shooting Logic ---
    private void HandleShooting()
    {
        // 1. Always look at the player when in range
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0; // Lock the rotation to the horizontal plane
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        // 2. Check if it's time to fire
        if (Time.time > nextFireTime)
        {
            // Reset the fire timer
            nextFireTime = Time.time + fireRate;

            // Instantiate the bullet
            GameObject bullet = Instantiate(bulletPrefab, shotSpawn.position, shotSpawn.rotation);

            // Assuming the bullet prefab has a Rigidbody and a script with a 'Launch' method
            // If you need the bullet to move, you'll need a simple Bullet script attached to the bulletPrefab
            // Example of launching the bullet (adjust speed as needed):
            // float bulletSpeed = 20f;
            // bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        }
    }
}