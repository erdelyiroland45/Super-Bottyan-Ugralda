using UnityEngine;

public class Bocs : MonoBehaviour
{
    [SerializeField] private float playerDamage = 1f; // Damage dealt to the player
    private int health = 5;                             // Bocs' health
    private int maxHealth = 5;                          // Maximum health
    [SerializeField] private int headDamage = 1;        // Damage taken by Bocs when stomped on
    [SerializeField] private BocsEletbar healthBar;     // Reference to the health bar
    [SerializeField] private GameObject projectilePrefab; // Prefab for projectiles
    [SerializeField] private float projectileLaunchForce = 5f; // Force for launching projectiles
    [SerializeField] private Transform spawnPoint;       // Spawn point for projectiles

    private Coroutine projectileSpawnCoroutine;          // Reference for the projectile spawning coroutine
    private bool isVisible = false;                       // Boolean to track visibility

    public int Health => health;                          // Public getter for current health
    public int MaxHealth => maxHealth;                    // Public getter for max health

    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Initialize health bar with current health
            healthBar.gameObject.SetActive(false); // Hide health bar initially
        }
    }

    private void OnBecameVisible()
    {
        isVisible = true;
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true); // Show health bar when visible
        }

        // Start projectile spawning coroutine
        if (projectileSpawnCoroutine == null)
        {
            projectileSpawnCoroutine = StartCoroutine(SpawnProjectilesCoroutine());
        }
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false); // Hide health bar when not visible
        }

        StopCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isVisible) // Check if Bocs is visible
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(playerDamage); // Apply damage to the player using Sebzodes
                Debug.Log("Damage dealt to player: " + playerDamage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isVisible) // Check if Bocs is visible
        {
            float playerYPosition = collision.transform.position.y;
            float bocsYPosition = transform.position.y;

            if (playerYPosition > bocsYPosition)
            {
                TakeDamage(headDamage); // Damage Bocs if stomped on
                Debug.Log("Bocs takes head stomp damage");

                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Adjust jump force as necessary
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isVisible) return; // Prevent damage if not visible

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Bocs took damage, remaining health: " + health);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Update health bar
        }

        if (health <= 0)
        {
            Destroy(gameObject); // Destroy Bocs when health is zero
            Debug.Log("Bocs destroyed");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false); // Hide health bar when destroyed
            }
            StopCoroutines();
        }
    }

    private void StopCoroutines()
    {
        if (projectileSpawnCoroutine != null)
        {
            StopCoroutine(projectileSpawnCoroutine);
            projectileSpawnCoroutine = null;
        }
    }

    private System.Collections.IEnumerator SpawnProjectilesCoroutine()
    {
        while (health > 0)
        {
            LaunchProjectile();
            yield return new WaitForSeconds(2f); // Change this value to adjust the interval for projectile launching
        }
    }

    private void LaunchProjectile()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Example launch direction: towards the player or fixed direction
                Vector2 launchDirection = new Vector2(-1, 0).normalized; // Launch to the left
                rb.AddForce(launchDirection * projectileLaunchForce, ForceMode2D.Impulse);
                Debug.Log("Projectile launched left.");
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab or spawn point is missing.");
        }
    }
}
