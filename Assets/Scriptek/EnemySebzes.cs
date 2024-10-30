using UnityEngine;

public class EnemySebzes : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // Amount of damage the enemy deals to the player

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that collided with the enemy is tagged as "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Attempt to get the player's health component
            Eletek playerHealth = collision.gameObject.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(damageAmount); // Apply damage to the player
                Debug.Log("Enemy dealt damage to player: " + damageAmount);
            }
        }
    }
}
