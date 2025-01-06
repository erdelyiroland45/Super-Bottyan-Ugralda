using UnityEngine;
using System.Collections;

public class Ivett : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f; // Damage dealt to the player
    private int health = 5;                       // Enemy health
    private int maxHealth = 5;                    // Max health
    [SerializeField] private int headDamage = 1;  // Damage taken when stomped by player
    [SerializeField] private IvettEletbar healthBar; // Reference to health bar
    [SerializeField] private Transform projectileSpawnPoint; // Projectile spawn point
    [SerializeField] private GameObject projectilePrefab; // Prefab for projectile
    [SerializeField] private float fireRate = 1.5f; // Frequency of projectile firing in seconds
    [SerializeField] private float projectileSpeed = 5f; // Speed of projectile
    [SerializeField] private Portalkezelo portalManager; // Reference to Portalkezelo

    private float nextFireTime = 0f;
    private bool isVisible = false;

    private bool invulnerable = false;  // To track invulnerability state
    private SpriteRenderer spriteRenderer;
    private Collider2D nonTriggerCollider;
    
    // Coroutine variable for projectile spawning
    private Coroutine projectileSpawnCoroutine = null;
    private Coroutine invulnerabilityCoroutine = null; // Coroutine for invulnerability blink effect

    public int Health => health;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on Ivett.");
        }

        // Find the non-trigger collider
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (!col.isTrigger)
            {
                nonTriggerCollider = col;
                break;
            }
        }

        if (nonTriggerCollider == null)
        {
            Debug.LogError("Non-trigger collider not found on Ivett.");
        }

        if (healthBar != null)
        {
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

        // Start projectile spawning coroutine when visible
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

        // Stop the projectile spawning coroutine when not visible
        StopCoroutines();
    }

    private void Update()
    {
        // Flip Ivett's sprite based on the player's position
        if (isVisible)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector2 directionToPlayer = player.transform.position - transform.position;

                // Flip the sprite depending on which side the player is
                if (directionToPlayer.x > 0)
                {
                    spriteRenderer.flipX = true; // Player is on the right
                }
                else
                {
                    spriteRenderer.flipX = false ; // Player is on the left
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isVisible && !invulnerable) // Check if Ivett is visible and not invulnerable
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Deal damage to the player
                Debug.Log("Damage dealt to player: " + sebzodes);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isVisible && !invulnerable) // Check if Ivett is visible and not invulnerable
        {
            float playerYPosition = collision.transform.position.y;
            float ivettYPosition = transform.position.y;

            if (playerYPosition > ivettYPosition)
            {
                // Apply damage to Ivett when stomped by the player
                TakeDamage(headDamage); // Head stomp damage
                Debug.Log("Ivett takes head stomp damage");

                // Apply upward force to the player (adjust jump force as needed)
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Adjust jump strength as needed
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable) return; // If Ivett is invulnerable, prevent damage

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Ivett took damage, remaining health: " + health);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Update health bar
        }

        if (health <= 0)
        {
            Destroy(gameObject); // Destroy Ivett when health is 0
            Debug.Log("Ivett destroyed");

            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false); // Hide health bar
            }
            StopCoroutines(); // Stop coroutines

            // Activate portal when Ivett is defeated
            if (portalManager != null)
            {
                portalManager.ActivatePortal();
            }
        }
        else
        {
            StartInvulnerability();
        }
    }

    private void StartInvulnerability()
    {
        invulnerable = true;
        if (nonTriggerCollider != null)
        {
            nonTriggerCollider.enabled = false; // Disable the non-trigger collider
        }

        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
        }

        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        float elapsed = 0f;
        while (elapsed < 3f) // Duration of invulnerability
        {
            // Blink effect (make Ivett semi-transparent)
            spriteRenderer.color = new Color(1, 1, 1, 0.1f);
            yield return new WaitForSeconds(0.2f); // Half blink interval

            spriteRenderer.color = Color.white; // Reset color
            yield return new WaitForSeconds(0.2f); // Half blink interval

            elapsed += 0.4f;
        }

        invulnerable = false;

        if (nonTriggerCollider != null)
        {
            nonTriggerCollider.enabled = true; // Re-enable the non-trigger collider
        }

        spriteRenderer.color = Color.white; // Ensure the sprite is fully visible
    }

    // Coroutine for projectile spawning (if needed)
    private IEnumerator SpawnProjectilesCoroutine()
    {
        while (isVisible)
        {
            // Shoot projectiles at the specified fire rate
            if (Time.time >= nextFireTime)
            {
                ShootProjectile();
                nextFireTime = Time.time + fireRate;
            }
            yield return null; // Wait for the next frame
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
            SpriteRenderer projectileSprite = projectile.GetComponent<SpriteRenderer>();

            if (rb != null && projectileCollider != null)
            {
                // Calculate the direction towards the player
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    Vector2 directionToPlayer = player.transform.position - transform.position;
                    directionToPlayer.Normalize(); // Normalize to get direction

                    // Set projectile velocity based on direction
                    rb.velocity = directionToPlayer * projectileSpeed;

                    // Flip the projectile's sprite based on Ivett's facing direction
                    if (spriteRenderer.flipX)
                    {
                        if (projectileSprite != null)
                        {
                            projectileSprite.flipX = true; // Flip the projectile to match Ivett's direction
                        }
                    }
                    else
                    {
                        if (projectileSprite != null)
                        {
                            projectileSprite.flipX = false; // Keep the projectile facing right
                        }
                    }
                }

                // Set collider as a trigger so it doesn't collide with non-player objects
                projectileCollider.isTrigger = true;
                
                // Add the OnTriggerEnter2D event handler directly to the projectile
                projectile.AddComponent<ProjectileCollision>().Initialize(sebzodes);
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab or spawn point is missing.");
        }
    }

    // Stop coroutines when Ivett is not visible
    private void StopCoroutines()
    {
        if (projectileSpawnCoroutine != null)
        {
            StopCoroutine(projectileSpawnCoroutine);
            projectileSpawnCoroutine = null;
        }

        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
            invulnerabilityCoroutine = null;
        }
    }

    // Projectile collision handling
    private class ProjectileCollision : MonoBehaviour
    {
        private float damage;

        public void Initialize(float damageValue)
        {
            damage = damageValue;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Interact only with the player
            if (collision.CompareTag("Player"))
            {
                Eletek playerHealth = collision.GetComponent<Eletek>();
                if (playerHealth != null)
                {
                    playerHealth.Sebzodes(damage); // Deal damage to the player
                    Debug.Log("Projectile dealt damage to player: " + damage);
                }
                Destroy(gameObject); // Destroy projectile on hit
            }
        }

        private void OnBecameInvisible()
        {
            // Destroy projectile if it leaves the screen
            Destroy(gameObject);
        }
    }
}
