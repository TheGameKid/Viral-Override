using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    // The amount of ammo this pickup grants
    [SerializeField] private int ammoRefillAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the Player
        // You should ensure your player GameObject is tagged "Player" in the Inspector.
        if (other.CompareTag("Player"))
        {
            // Get the Player script component from the colliding object
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                // Call the new method in the Player script to add ammo
                player.AddAmmo(ammoRefillAmount);

                // Destroy the ammo pickup object
                Destroy(gameObject);
            }
        }
    }
}