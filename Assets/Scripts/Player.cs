using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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


    private void FixedUpdate()
    {
        if (health > 0)
        {
            CubeTranslation();
        }
    }

    public void CubeTranslation()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        Vector2 mouseDelta = mouse.delta.ReadValue();

        if (kb == null) return;

        if (Gamepad.current != null)
        {
            var gp = Gamepad.current;

            if (gp.leftStick.up.isPressed) VZ = 0.1f;
            if (gp.leftStick.down.isPressed) VZ = -0.1f;

            if (gp.leftStick.left.isPressed) VX = 0.1f;
            if (gp.leftStick.right.isPressed) VX = -0.1f;

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

        Vector3 p = transform.position;

        // --- NEW: move on ground-plane only (ignore vertical from pitch) ---
        Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 rightOnPlane = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        // -------------------------------------------------------------------

        if (VZ != 0)
        {
            // p += VZ * transform.forward;           // (old)
            p += VZ * forwardOnPlane;                 // (ground-plane)
        }
        if (VX != 0)
        {
            // p += VX * -transform.right;            // (old)
            p += VX * -rightOnPlane;                  // (ground-plane)
        }

        // --- Clamp X rotation and lock Z before updating position ---
        {
            Vector3 e = transform.eulerAngles;
            float x = (e.x > 180f) ? e.x - 360f : e.x;
            const float PITCH_MIN = -45f;
            const float PITCH_MAX = 30f;
            x = Mathf.Clamp(x, PITCH_MIN, PITCH_MAX);
            transform.eulerAngles = new Vector3(x, e.y, 0f);   // keep Z = 0
        }
        // ------------------------------------------------------------

        transform.position = p;

        VZ = VX = rotX = rotY = 0;


    }

}