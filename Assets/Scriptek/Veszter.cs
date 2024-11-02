using UnityEngine;

public class Veszter : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f;  // Damage applied to the player
    private int health = 5;                        // Enemy HP
    private int maxHealth = 5;                     // Maximum health
    [SerializeField] private int headDamage = 1;   // Damage Veszter takes when the player jumps on its head
    [SerializeField] private VeszterEletbar healthBar; // Reference to the health bar
    [SerializeField] private GameObject filamentPrefab; // Prefab for the Filament projectile
    [SerializeField] private Transform spawnPoint; // Spawn location for the filament

    public int Health => health;                   // Public getter for current health
    public int MaxHealth => maxHealth;             // Public getter for max health

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Damage the player
                Debug.Log("Damage dealt to player: " + sebzodes);
            }
        }
    }

    // Detects collisions with the "HeadCollider" to apply damage to Veszter
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the player is above Veszter (jumping on its head)
            float playerYPosition = collision.transform.position.y;
            float veszterYPosition = transform.position.y;

            if (playerYPosition > veszterYPosition)
            {
                TakeDamage(headDamage); // Apply damage to Veszter
                Debug.Log("Veszter takes head stomp damage");

                // Optionally, add a small bounce effect to the player
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Adjust bounce force as needed
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // Decrease health
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth); // Ensure health doesn't go below zero
        Debug.Log("Enemy took damage, remaining health: " + health);

        // Notify the health bar to update
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Update the health bar to reflect current health
        }

        // Check if Veszter is dead
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy if health reaches zero
            Debug.Log("Enemy destroyed");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false); // Hide the health bar when Veszter is destroyed
            }
        }
        else
        {
            // Spawn a filament projectile when taking damage (optional)
            SpawnFilament();
        }
    }

    private void SpawnFilament()
    {
        if (filamentPrefab != null && spawnPoint != null)
        {
            Instantiate(filamentPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Filament spawned successfully.");
        }
        else
        {
            Debug.LogWarning("Filament prefab or spawn point is missing.");
        }
    }
}
