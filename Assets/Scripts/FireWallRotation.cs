using UnityEngine;

public class FireWallRotation : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Player to orbit around

    [Header("Orbit Settings")]
    public float orbitSpeed = 200f;    // Degrees per second

    void OnEnable()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        // We do NOT move or record position here.
        // The shield starts exactly where you placed it in the scene.
    }

    void Update()
    {
        if (player == null) return;

        // Orbit around the player horizontally (around world Y axis)
        transform.RotateAround(player.position, -Vector3.up, orbitSpeed * Time.deltaTime);

        // Optional: if you want the shield to always face outward from the player,
        // uncomment this line:
        // transform.rotation = Quaternion.LookRotation(transform.position - player.position, Vector3.up);
    }
}


