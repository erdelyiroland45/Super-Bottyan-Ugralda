using UnityEngine;

public class Veszter : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f; // Damage amount to be applied to the player
    private int health = 5; // Enemy HP

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (collision.CompareTag("Player"))
        {
            // Get the Eletek component from the player and apply damage
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Call the damage method
                Debug.Log("Damage dealt to player: " + sebzodes); // Log for debugging
            }
        }
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        health -= damage; // Reduce health by damage amount
        Debug.Log("Enemy took damage, remaining health: " + health);
        
        // Check if the enemy has died
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy enemy if health is zero
            Debug.Log("Enemy destroyed");
        }
    }
}
