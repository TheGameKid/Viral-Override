using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // <--- ADDED THIS

public class Player : MonoBehaviour
{
    // --- NEW FIELDS FOR CHARACTER CONTROLLER ---
    private CharacterController _controller;
    private Vector3 _velocity; // Stores vertical velocity (gravity/jump)
    // Exposed speed and gravity for tuning in the Inspector
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float gravity = -20f;
    // -------------------------------------------

    // --- FALL DETECTION FIELD ---
    [Header("Game Over Settings")]
    [Tooltip("The Y-coordinate below which the player restarts the game.")]
    [SerializeField] private float fallHeightLimit = -20f; // Adjust in Inspector
    // --------------------------

    float VZ = 0f, VX = 0f;
    public float rotX, rotY;
    public GameObject bullet;
    public GameObject ShotSpawn;
    public GameObject HUD;
    public GameObject Firewall;
    public GameObject Firewall2;
    public GameObject FirewallEffect;
    public GameObject Encryptor;
    public GameObject Decoder;
    public TextMeshProUGUI FireWallCooldownText;
    public TextMeshProUGUI EncryptorCooldownText;
    public TextMeshProUGUI DecoderCooldownText;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI AmmoCountText;
    public Image HealthBarFill;
    public Image HealthBarForeground;
    public Image AmmoFill;
    public Image AmmoForeground;
    public Image FireWallBox;
    public Image EncryptorBox;
    public Image DecoderBox;
    public GameObject Fire;
    public int FireWallCooldownTimer;
    public int EncryptorCooldownTimer;
    public int DecoderCooldownTimer;
    public float FireWallTimer;
    public float EncryptorTimer;
    public float DecoderTimer;
    public float timer1;
    public float timer2;
    public float timer3;
    public int health;
    public int maxHealth;
    public int ammo;
    public int maxAmmo;
    public bool FireWallUp;
    public bool EncryptorUp;
    public bool DecoderUp;
    public Color specialBlue = new Color(7, 157, 242, 255);
    public Game game;


    void Awake()
    {
        // --- CHARACTER CONTROLLER INITIALIZATION ---
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
        {
            Debug.LogError("Player script requires a CharacterController component.");
        }
        // -------------------------------------------

        health = 100;
        maxHealth = 100;
        ammo = 30;
        maxAmmo = 30;
        timer1 = 0;
        timer2 = 0;
        timer3 = 0;
        FireWallCooldownText.text = "";
        DecoderCooldownText.text = "";
        EncryptorCooldownText.text = "";
        FireWallUp = false;
        EncryptorUp = false;
        DecoderUp = false;
        FireWallBox.color = Color.red;
        DecoderBox.color = Color.green;
        EncryptorBox.color = specialBlue;
    }

    // Update is called once per frame
    void Update()
    {
        // --- NEW: FALL DETECTION ---
        if (transform.position.y < fallHeightLimit)
        {
            RestartGame();
        }
        // ---------------------------

        UpdateHealth((float)health / (float)maxHealth);
        UpdateAmmo((float)ammo / (float)maxAmmo);

        if (FireWallUp)
        {
            if (FireWallTimer > 0)
            {
                FireWallTimer -= Time.deltaTime;
                FireWallBox.color = Color.black;
            }
            else
            {
                FireWallTimer = 0;
                Firewall.SetActive(false);
                Firewall2.SetActive(false);
                FirewallEffect.SetActive(false);
                if (timer1 <= 1)
                {
                    timer1 += Time.deltaTime;
                }
                else
                {
                    timer1 = 0;
                    FireWallCooldownTimer -= 1;

                }

                if (FireWallCooldownTimer != 0)
                {
                    FireWallCooldownText.text = FireWallCooldownTimer.ToString();
                }
                else
                {
                    FireWallCooldownTimer = 0;
                    FireWallCooldownText.text = "";
                    FireWallBox.color = Color.red;
                    Fire.SetActive(true);

                    FireWallUp = false;
                }
            }

        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void TakeAmmo(int ammo)
    {
        this.ammo -= ammo;
    }

    public void UpdateHealth(float fraction)
    {
        HealthBarFill.fillAmount = fraction;

        if (health >= 75)
        {
            HealthBarFill.color = Color.green;
            HPText.color = Color.white;
        }
        else if (health >= 50)
        {
            HealthBarFill.color = Color.yellow;
            HPText.color = Color.yellow;
        }
        else if (health >= 25)
        {
            HealthBarFill.color = Color.orange;
            HPText.color = Color.orange;
        }
        else
        {
            HealthBarFill.color = Color.red;
            HPText.color = Color.red;
        }

        HPText.text = "HP: " + health + "/" + maxHealth;
    }

    public void UpdateAmmo(float fraction)
    {
        AmmoFill.fillAmount = fraction;
        AmmoCountText.text = "AMMO: " + ammo + "/" + maxAmmo;
    }

    // --- FixedUpdate for Physics and Gravity ---
    private void FixedUpdate()
    {
        if (health > 0)
        {
            CubeTranslation();
        }

        // 1. Gravity and Grounding (Prevents Bouncing on floor/walls)
        if (_controller.isGrounded)
        {
            // Crucial: Set a small negative value to keep the player firmly grounded
            // and prevent vertical jitters/bounces from unstable physics.
            _velocity.y = -2f;
        }
        else
        {
            // Apply gravity over time
            _velocity.y += gravity * Time.deltaTime;
        }

        // 2. Apply vertical movement (gravity)
        _controller.Move(_velocity * Time.deltaTime);
    }

    public void CubeTranslation()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        Vector2 mouseDelta = mouse.delta.ReadValue();

        if (kb == null) return;

        // ... (Your Input and Rotation Logic) ...
        if (Gamepad.current != null)
        {
            var gp = Gamepad.current;
            // Gamepad movement and rotation logic...
            if (gp.leftStick.up.isPressed) VZ = 0.1f;
            if (gp.leftStick.down.isPressed) VZ = -0.1f;
            if (gp.leftStick.left.isPressed) VX = 0.1f;
            if (gp.leftStick.right.isPressed) VX = -0.1f;
            // ... (rest of gamepad code) ...
            if ((gp.rightStick.up.isPressed) && transform.rotation.x > -45f)
            {
                rotX -= 0.1f;
                transform.Rotate(rotX * 10.0f, 0, 0);
            }
            if ((gp.rightStick.down.isPressed) && transform.rotation.x < 30f)
            {
                rotX += 0.1f;
                transform.Rotate(rotX * 10.0f, 0, 0);
            }
            if ((gp.rightStick.left.isPressed))
            {
                rotY -= 0.1f;
                transform.Rotate(0, rotY * 20.0f, 0);

            }
            if (gp.rightStick.right.isPressed)
            {
                rotY += 0.1f;
                transform.Rotate(0, rotY * 20.0f, 0);
            }

            if (gp.rightShoulder.wasPressedThisFrame && ammo > 0)
            {
                Instantiate(bullet, ShotSpawn.transform.position, transform.rotation);
                TakeAmmo(1);
                game.shoot.Play();
            }

            if (gp.startButton.wasPressedThisFrame)
            {

                game.HUD.SetActive(false);
                game.PauseMenu.SetActive(true);

                Time.timeScale = 0;
                game.pause.Play();
            }

            if (gp.squareButton.wasPressedThisFrame && FireWallCooldownTimer == 0)
            {
                FireWallTimer = 10;
                FireWallCooldownTimer = 10;
                Fire.SetActive(false);
                Firewall.SetActive(true);
                Firewall2.SetActive(true);
                FirewallEffect.SetActive(true);
                FireWallUp = true;
            }
            if (gp.triangleButton.wasPressedThisFrame && EncryptorCooldownTimer == 0)
            {
                DecoderUp = true;
            }
            if (gp.circleButton.wasPressedThisFrame && DecoderCooldownTimer == 0)
            {
                EncryptorUp = true;
            }
            if (gp.crossButton.wasPressedThisFrame && game.canEnterDoor)
            {
                StartCoroutine(game.DoorTransitionRoutine());
            }
        }

        if (kb.upArrowKey.isPressed) VZ = 0.1f;
        if (kb.downArrowKey.isPressed) VZ = -0.1f;
        if (kb.leftArrowKey.isPressed) VX = 0.1f;
        if (kb.rightArrowKey.isPressed) VX = -0.1f;

        // Mouse rotation logic...
        if ((mouseDelta.y > 0) && transform.rotation.x > -45f)
        {
            rotX -= 0.1f;
            transform.Rotate(rotX * 10.0f, 0, 0);
        }
        if ((mouseDelta.y < 0) && transform.rotation.x < 30f)
        {
            rotX += 0.1f;
            transform.Rotate(rotX * 10.0f, 0, 0);
        }
        if ((mouseDelta.x < 0))
        {
            rotY -= 0.1f;
            transform.Rotate(0, rotY * 20.0f, 0);

        }
        if (mouseDelta.x > 0)
        {
            rotY += 0.1f;
            transform.Rotate(0, rotY * 20.0f, 0);
        }

        // Mouse shoot and ability code...
        if (mouse.leftButton.wasPressedThisFrame && ammo > 0)
        {
            Instantiate(bullet, ShotSpawn.transform.position, transform.rotation);
            TakeAmmo(1);
            game.shoot.Play();
        }
        if (kb.digit1Key.wasPressedThisFrame && FireWallCooldownTimer == 0)
        {
            FireWallTimer = 10;
            FireWallCooldownTimer = 10;
            FireWallUp = true;
        }
        if (kb.digit3Key.wasPressedThisFrame && EncryptorCooldownTimer == 0)
        {
            EncryptorUp = true;
        }
        if (kb.digit2Key.wasPressedThisFrame && DecoderCooldownTimer == 0)
        {
            DecoderUp = true;
        }
        if (kb.escapeKey.wasPressedThisFrame)
        {
            game.HUD.SetActive(false);
            game.pause.Play();
            game.PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        if (kb.enterKey.wasPressedThisFrame && game.canEnterDoor)
        {
            StartCoroutine(game.DoorTransitionRoutine());
        }

        // --- HORIZONTAL MOVEMENT FIX ---
        // Calculate movement direction
        Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 rightOnPlane = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 moveDirection = Vector3.zero;

        if (VZ != 0)
        {
            moveDirection += VZ * forwardOnPlane;
        }
        if (VX != 0)
        {
            moveDirection += VX * -rightOnPlane;
        }

        if (moveDirection.magnitude > 0)
        {
            // Use normalized direction to prevent faster diagonal movement
            Vector3 finalMove = moveDirection.normalized * moveSpeed * Time.deltaTime;

            // Use CharacterController.Move for collision-aware horizontal movement
            _controller.Move(finalMove);
        }
        // ---------------------------------------


        // --- Clamp X rotation and lock Z (Prevents Spinning/Tilting) ---
        {
            Vector3 e = transform.eulerAngles;
            float x = (e.x > 180f) ? e.x - 360f : e.x;
            const float PITCH_MIN = -45f;
            const float PITCH_MAX = 30f;
            x = Mathf.Clamp(x, PITCH_MIN, PITCH_MAX);
            // Locking Z to 0 prevents the player from rotating/spinning on the Z axis
            transform.eulerAngles = new Vector3(x, e.y, 0f);
        }
        // ------------------------------------------------------------

        VZ = VX = rotX = rotY = 0;
    }

    // --- NEW: RESTART GAME FUNCTION ---
    private void RestartGame()
    {
        // Get the index of the currently active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Check for Time.timeScale being 0 (paused) and reset it
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    }
    // ------------------------------------
}