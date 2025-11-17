using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
<<<<<<< Updated upstream
    bool MoveLeft = false, MoveRight = false, MoveForward = false, MoveBackward = false;
    bool RotateLeft = false, RotateRight = false;
=======
    // *** NEW: Add CharacterController reference ***
    private CharacterController characterController;

    // Add a movement speed variable for better control
    public float movementSpeed = 5.0f; // Adjust this value in the Inspector

>>>>>>> Stashed changes
    float VZ = 0f, VX = 0f;
    public float rotX, rotY;
    public GameObject bullet;
    public GameObject ShotSpawn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
<<<<<<< Updated upstream
        
=======
        // *** NEW: Get the CharacterController component ***
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("Player script requires a CharacterController component!");
        }

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
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
      
=======
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

>>>>>>> Stashed changes
    }

    private void FixedUpdate()
    {
        CubeTranslation();
    }

    public void CubeTranslation()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;
        var mouse = Mouse.current;

        Vector2 mouseDelta = mouse.delta.ReadValue();

        if (kb == null && gp == null) return;

<<<<<<< Updated upstream
        if (kb.upArrowKey.isPressed || gp.leftStick.up.isPressed) VZ = 0.1f;
        if (kb.downArrowKey.isPressed || gp.leftStick.down.isPressed) VZ = -0.1f;

        if (kb.leftArrowKey.isPressed || gp.leftStick.left.isPressed) VX = 0.1f;
        if (kb.rightArrowKey.isPressed || gp.leftStick.right.isPressed) VX = -0.1f;
=======
        // Reset velocity variables at the start of the frame
        float VZ_temp = 0f;
        float VX_temp = 0f;

        // --- Input Reading (Updated to use VZ_temp/VX_temp) ---

        if (Gamepad.current != null)
        {
            var gp = Gamepad.current;

            if (gp.leftStick.up.isPressed) VZ_temp = 1f; // Use 1f instead of 0.1f and multiply by speed later
            if (gp.leftStick.down.isPressed) VZ_temp = -1f;

            if (gp.leftStick.left.isPressed) VX_temp = 1f;
            if (gp.leftStick.right.isPressed) VX_temp = -1f;

            // Rotation logic remains the same (using transform.Rotate is fine for camera/view rotation)
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

        // Keyboard Input (Updated to use VZ_temp/VX_temp)
        if (kb.upArrowKey.isPressed) VZ_temp = 1f;
        if (kb.downArrowKey.isPressed) VZ_temp = -1f;
>>>>>>> Stashed changes

        if (kb.leftArrowKey.isPressed) VX_temp = 1f;
        if (kb.rightArrowKey.isPressed) VX_temp = -1f;

<<<<<<< Updated upstream
        if ((gp.rightStick.up.isPressed || mouseDelta.y > 0) && transform.rotation.x > -45f)
=======
        // Mouse rotation logic...
        if ((mouseDelta.y > 0) && transform.rotation.x > -45f)
>>>>>>> Stashed changes
        {
            rotX -= 0.1f;
            transform.Rotate(rotX * 10.0f, 0, 0);
        }
        if ((gp.rightStick.down.isPressed || mouseDelta.y < 0) && transform.rotation.x < 30f)
        {
            rotX += 0.1f;
            transform.Rotate(rotX * 10.0f, 0, 0);
        }
        if ((gp.rightStick.left.isPressed || mouseDelta.x < 0))
        {
            rotY -= 0.1f;
            transform.Rotate(0, rotY * 20.0f, 0);

        }
        if (gp.rightStick.right.isPressed || mouseDelta.x > 0)
        {
            rotY += 0.1f;
            transform.Rotate(0, rotY * 20.0f, 0);
        }

        if (gp.rightShoulder.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame)
        {
            Instantiate(bullet, ShotSpawn.transform.position, transform.rotation);
        }


        // --- Calculate Movement Vector (NO MORE DIRECT P ASSIGNMENT) ---

        // This is necessary to reset VZ and VX for the next frame
        VZ = VZ_temp;
        VX = VX_temp;

        Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 rightOnPlane = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        // Calculate the movement vector
        Vector3 move = (VZ * forwardOnPlane) + (VX * -rightOnPlane);

        // Normalize the vector if moving diagonally to prevent faster diagonal movement
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        // Apply speed and Time.deltaTime (or Time.fixedDeltaTime since it's in FixedUpdate)
        // Using fixedDeltaTime for consistency with FixedUpdate
        move *= movementSpeed * Time.fixedDeltaTime;

        // *** THE FIX: Use CharacterController.Move() ***
        if (characterController != null)
        {
            // CharacterController.Move handles collision gracefully, preventing the bounce
            characterController.Move(move);
        }

        // --- Clamp X rotation and lock Z ---
        {
            Vector3 e = transform.eulerAngles;
            float x = (e.x > 180f) ? e.x - 360f : e.x;
            const float PITCH_MIN = -45f;
            const float PITCH_MAX = 30f;
            x = Mathf.Clamp(x, PITCH_MIN, PITCH_MAX);
            transform.eulerAngles = new Vector3(x, e.y, 0f);    // keep Z = 0
        }
<<<<<<< Updated upstream
        // ------------------------------------------------------------

        transform.position = p;

        MoveForward = MoveRight = MoveLeft = MoveBackward = false;
        RotateLeft = RotateRight = false;
        VZ = VX = rotX = rotY = 0;

=======
        // ------------------------------------
>>>>>>> Stashed changes

        // Reset rotation variables here if transform.Rotate accumulates them
        rotX = rotY = 0;
    }

}
