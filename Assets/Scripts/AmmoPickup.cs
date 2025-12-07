using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    // The amount of ammo this pickup grants
    [SerializeField] private int ammoRefillAmount = 10;

    [Header("Audio")]
    [Tooltip("The sound clip to play when the item is picked up.")]
    public AudioClip pickupSound; // Reference to the audio clip file

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the Player
        if (other.CompareTag("Player"))
        {
            // Get the Player script component from the colliding object
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                // 1. Give the player the ammo
                player.AddAmmo(ammoRefillAmount);

                // 2. Play the pickup sound
                if (pickupSound != null)
                {
                    // Plays the clip once at the pickup's position.
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // 3. Destroy the ammo pickup object
                Destroy(gameObject);
            }
        }
    }
}