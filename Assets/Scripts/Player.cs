using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    bool MoveLeft = false, MoveRight = false, MoveForward = false, MoveBackward = false;
    bool RotateLeft = false, RotateRight = false;
    float VZ = 0f, VX = 0f;
    public float rotX, rotY;
    public GameObject bullet;
    public GameObject ShotSpawn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

        if (kb.upArrowKey.isPressed || gp.leftStick.up.isPressed) VZ = 0.1f;
        if (kb.downArrowKey.isPressed || gp.leftStick.down.isPressed) VZ = -0.1f;

        if (kb.leftArrowKey.isPressed || gp.leftStick.left.isPressed) VX = 0.1f;
        if (kb.rightArrowKey.isPressed || gp.leftStick.right.isPressed) VX = -0.1f;


        if ((gp.rightStick.up.isPressed || mouseDelta.y > 0) && transform.rotation.x > -45f)
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

        MoveForward = MoveRight = MoveLeft = MoveBackward = false;
        RotateLeft = RotateRight = false;
        VZ = VX = rotX = rotY = 0;


    }

}