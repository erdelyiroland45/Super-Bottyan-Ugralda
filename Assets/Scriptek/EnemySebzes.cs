using UnityEngine;

public class EnemySebzes : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f; // Amount of damage the enemy deals to the player

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            // Try to get the Eletek component from the player
            Eletek playerHealth = collision.gameObject.GetComponent<Eletek>();

            if (playerHealth != null)
            {
                // Apply damage if playerHealth is found
                playerHealth.Sebzodes(damageAmount);
                Debug.Log("Enemy dealt damage to player: " + damageAmount);
            }
            else
            {
                Debug.LogWarning("Eletek component not found on the Player object. Please ensure the Player has the Eletek component attached.");
            }
        }
    }
}
