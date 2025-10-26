using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // These boolean flags are unused in your logic, so they're kept but not actively maintained.
    bool MoveLeft = false, MoveRight = false, MoveForward = false, MoveBackward = false;
    bool RotateLeft = false, RotateRight = false;

    float VZ = 0f, VX = 0f;
    // rotX/rotY are public in the original, but no longer used for accumulation.
    public float rotX, rotY;

    // Constants for fixed speed/rotation
    private const float MoveSpeed = 0.1f;
    private const float RotationDelta = 0.5f;
    private const float PITCH_MIN = -45f;
    private const float PITCH_MAX = 30f;


    void FixedUpdate()
    {
        CubeTranslation();
    }

    public void CubeTranslation()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        // Reset movement vectors at the start of the frame
        VZ = 0f;
        VX = 0f;

        // --- 1. Movement Input (Fixes NullReferenceException) ---
        // Use '?.isPressed == true' to safely check if the device and the key exist.

        // Forward/Backward Movement
        if (kb?.upArrowKey.isPressed == true || gp?.leftStick.up.isPressed == true) VZ = MoveSpeed;
        if (kb?.downArrowKey.isPressed == true || gp?.leftStick.down.isPressed == true) VZ = -MoveSpeed;

        // Strafe Left/Right Movement
        if (kb?.leftArrowKey.isPressed == true || gp?.leftStick.left.isPressed == true) VX = MoveSpeed;
        if (kb?.rightArrowKey.isPressed == true || gp?.leftStick.right.isPressed == true) VX = -MoveSpeed;


        // --- 2. Rotation Input (Fixes uncontrolled spinning) ---
        // We only proceed if a gamepad is connected to avoid errors on 'gp.rightStick'.
        if (gp != null)
        {
            // Pitch (Up/Down)
            if (gp.rightStick.up.isPressed)
            {
                // Apply small, fixed rotation directly instead of accumulating in rotX
                transform.Rotate(-RotationDelta, 0, 0, Space.Self);
            }
            if (gp.rightStick.down.isPressed)
            {
                transform.Rotate(RotationDelta, 0, 0, Space.Self);
            }

            // Yaw (Left/Right)
            if (gp.rightStick.left.isPressed)
            {
                transform.Rotate(0, -RotationDelta * 2.0f, 0, Space.World);
            }
            if (gp.rightStick.right.isPressed)
            {
                transform.Rotate(0, RotationDelta * 2.0f, 0, Space.World);
            }
        }
        // Note: The manual rotation boundary checks (e.g., transform.rotation.x > -45f) 
        // are redundant and flawed because they check quaternions. The clamping step (below)
        // is the correct way to enforce limits on the Euler angles.


        // --- 3. Position Update ---
        Vector3 p = transform.position;

        // Project movement vectors onto the flat ground plane
        Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 rightOnPlane = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        if (VZ != 0)
        {
            p += VZ * forwardOnPlane;
        }
        if (VX != 0)
        {
            p += VX * -rightOnPlane;
        }

        // --- 4. Clamp Pitch Rotation and Lock Z ---
        {
            Vector3 e = transform.eulerAngles;
            // Convert the X angle to a signed value (-180 to 180) before clamping
            float x = (e.x > 180f) ? e.x - 360f : e.x;

            x = Mathf.Clamp(x, PITCH_MIN, PITCH_MAX);

            // Apply the clamped angle and ensure Z rotation is 0
            transform.eulerAngles = new Vector3(x, e.y, 0f);
        }

        transform.position = p;

        // --- 5. Clean up (Remove redundant resets) ---
        // Removed VZ = VX = rotX = rotY = 0; from the end since VZ/VX are reset at the top
        // and rotX/rotY are no longer used for accumulation.
        MoveForward = MoveRight = MoveLeft = MoveBackward = false;
        RotateLeft = RotateRight = false;
    }
}