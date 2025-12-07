using UnityEngine;

public class FloatingHeadEnemy : MonoBehaviour
{
    // --- Public Variables (Set in Inspector) ---
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

    // ** NEW HORIZONTAL MOVEMENT VARIABLES **
    [Header("Horizontal Movement")]
    public float moveSpeed = 3f; // Speed of horizontal movement
    public float patrolRadius = 10f; // Max distance from start to patrol
    public float moveChangeTime = 4f; // How often the head chooses a new destination

    // --- Private Variables ---
    private float nextFireTime;
    private Vector3 startPosition;
    private Vector3 targetPosition; // The new horizontal movement target
    private float nextMoveChangeTime;

    void Start()
    {
        startPosition = transform.position;
        nextFireTime = Time.time;
        nextMoveChangeTime = Time.time; // Initialize move change time

        // Initial setup for the first target position
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
    }

    void Update()
    {
        if (player == null) return;

        // 1. Move Horizontally (X and Z)
        ApplyHorizontalMovement();

        // 2. Hover Vertically (Y)
        ApplyHoverMovement();

        // 3. Face the Player (Rotation is now decoupled from movement)
        RotateToFacePlayer();

        // 4. Fire Bullets
        CheckAndFire();

        firePoint.LookAt(player);
    }

    // --- Core Logic Methods ---

    private void RotateToFacePlayer()
    {
        // Calculate the direction vector from the enemy to the player
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the rotation on the horizontal plane

        // Create the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void ApplyHoverMovement()
    {
        // Calculate the new Y position using the startPosition (X and Z are handled separately)
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        // Only update the Y position here, keeping X and Z as set by HorizontalMovement
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // ** NEW HORIZONTAL MOVEMENT IMPLEMENTATION **
    private void ApplyHorizontalMovement()
    {
        // Check if it's time to choose a new target position
        if (Time.time > nextMoveChangeTime || Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            ChooseNewTargetPosition();
        }

        // Calculate the direction vector for horizontal movement
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Apply movement only on the X and Z axes
        Vector3 horizontalMove = moveDirection * moveSpeed * Time.deltaTime;
        horizontalMove.y = 0; // Ensure no vertical movement from this step

        // Apply the horizontal movement
        transform.position += horizontalMove;
    }

    private void ChooseNewTargetPosition()
    {
        // Set the next time to change the movement target
        nextMoveChangeTime = Time.time + moveChangeTime;

        // Calculate a random point within the patrol radius centered around the enemy's start position
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;

        // Ensure the new target position is on the ground plane (only X and Z matter)
        targetPosition = new Vector3(randomDirection.x, transform.position.y, randomDirection.z);
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
        // Instantiation and launching logic remains the same
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}