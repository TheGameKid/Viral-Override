using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Amount of health to restore to the player.")]
    public int healthRestoreAmount = 25;

    // Optional: Add an effect/sound reference here if needed

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

            // 2. Update the UI immediately (call the public function on the Player script)
            // Note: You must update the player's UI after changing health
            playerScript.UpdateHealth((float)playerScript.health / (float)playerScript.maxHealth);

            // Optional: Play a pickup sound or show a particle effect here

            // 3. Destroy the pickup item so it can't be reused
            Destroy(gameObject);
        }
    }
}