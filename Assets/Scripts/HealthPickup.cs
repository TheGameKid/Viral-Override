using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Amount of health to restore to the player.")]
    public int healthRestoreAmount = 25;

    [Header("Audio Settings")]
    [Tooltip("The sound clip to play when the item is picked up.")]
    public AudioClip pickupSound; // Reference to the audio clip file

    // This function is called when another object (Collider) enters this object's trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the Player script
        Player playerScript = other.GetComponent<Player>();

        if (playerScript != null)
        {
            // 1. Restore the player's health
            playerScript.health += healthRestoreAmount;

            // Ensure health does not exceed maxHealth
            if (playerScript.health > playerScript.maxHealth)
            {
                playerScript.health = playerScript.maxHealth;
            }

            // 2. Update the UI immediately
            playerScript.UpdateHealth((float)playerScript.health / (float)playerScript.maxHealth);

            // 3. Play the pickup sound
            if (pickupSound != null)
            {
                // Plays the clip once at the pickup's position.
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // 4. Destroy the pickup item so it can't be reused
            Destroy(gameObject);
        }
    }
}