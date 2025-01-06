using UnityEngine;
using System.Collections;

public class Veszter : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f;             // Damage dealt to the player
    private int health = 5;                                   // Veszter's health
    private int maxHealth = 5;                                // Maximum health
    [SerializeField] private int headDamage = 1;              // Damage taken by Veszter when stomped on
    [SerializeField] private VeszterEletbar healthBar;        // Reference to the health bar
    [SerializeField] private GameObject projectilePrefab;     // Prefab for projectiles
    [SerializeField] private float projectileLaunchForce = 5f; // Force for launching projectiles
    [SerializeField] private Transform spawnPoint;            // Spawn point for projectiles
    [SerializeField] private Transform filamentdobáshely; // Új spawn pont a projektilokhoz
    [SerializeField] private GameObject[] filamentPrefabs;    // Array of filament prefabs
    [SerializeField] private Transform keySpawnPoint;         // Spawn point for the key
    [SerializeField] private GameObject keyPrefab;            // Prefab for the key
    [SerializeField] private Portalkezelo portalManager;      // Reference to the Portalkezelo (Portal Manager)
    [SerializeField] private float spawnDelay = 3f;           // Delay between filament spawns
    [SerializeField] private float invulnerabilityDuration = 3f; // Duration of invulnerability
    [SerializeField] private float blinkInterval = 0.2f;      // Interval for blinking effect

    private SpriteRenderer spriteRenderer;                    // Reference to the SpriteRenderer for blinking
    private Collider2D nonTriggerCollider;                    // Non-trigger collider for handling invulnerability
    private Coroutine projectileSpawnCoroutine;
    private Coroutine filamentSpawnCoroutine;
    private bool isVisible = false;                           // Boolean to track visibility
    private bool invulnerable = false;                        // Tracks if Veszter is invulnerable
    private bool isIdle = false;                              // Flag to indicate if Veszter is idle
    private int stompCount = 0;                               // Counter for stomps on Veszter

    public int Health => health;                              // Public getter for current health
    public int MaxHealth => maxHealth;                        // Public getter for max health

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on Veszter.");
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
            Debug.LogError("Non-trigger collider not found on Veszter.");
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
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

        if (!isIdle)
        {
            if (projectileSpawnCoroutine == null)
            {
                projectileSpawnCoroutine = StartCoroutine(SpawnProjectilesCoroutine());
            }
            if (filamentSpawnCoroutine == null && filamentPrefabs.Length > 0)
            {
                filamentSpawnCoroutine = StartCoroutine(SpawnFilamentsCoroutine());
            }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isVisible) // Check if Veszter is visible
        {
            float playerYPosition = collision.transform.position.y;
            float veszterYPosition = transform.position.y;

            if (playerYPosition > veszterYPosition)
            {
                stompCount++; // Increment the stomp count
                TakeDamage(headDamage); // Apply damage to Veszter
                Debug.Log("Veszter takes head stomp damage");

                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Adjust jump force
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isVisible || invulnerable) return; // Prevent damage if not visible or invulnerable

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Enemy took damage, remaining health: " + health);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Update the health bar
        }

        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy
            Debug.Log("Enemy destroyed");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false); // Hide the health bar
            }
            StopCoroutines();

            // Activate portal and spawn key
            if (portalManager != null)
            {
                portalManager.ActivatePortal();
            }
            SpawnKey();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator SpawnFilamentsCoroutine()
    {
        while (health > 0 && !isIdle)
        {
            SpawnFilament();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnFilament()
    {
        if (filamentdobáshely != null && filamentPrefabs.Length > 0)
        {
            GameObject chosenFilament = filamentPrefabs[Random.Range(0, filamentPrefabs.Length)];
            GameObject spawnedFilament = Instantiate(chosenFilament, filamentdobáshely.position, Quaternion.identity);

            // Ensure filament ignores Veszter's colliders
            Collider2D[] veszterColliders = GetComponents<Collider2D>();
            Collider2D filamentCollider = spawnedFilament.GetComponent<Collider2D>();
            if (filamentCollider != null)
            {
                foreach (Collider2D veszterCollider in veszterColliders)
                {
                    Physics2D.IgnoreCollision(veszterCollider, filamentCollider);
                }
            }
            Debug.Log("Filament spawned.");
        }
    }

    private void SpawnKey()
    {
        if (keyPrefab != null && keySpawnPoint != null)
        {
            Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);
            Debug.Log("Key spawned.");
        }
    }

    private void StopCoroutines()
    {
        if (projectileSpawnCoroutine != null)
        {
            StopCoroutine(projectileSpawnCoroutine);
            projectileSpawnCoroutine = null;
        }
        if (filamentSpawnCoroutine != null)
        {
            StopCoroutine(filamentSpawnCoroutine);
            filamentSpawnCoroutine = null;
        }
    }

    private IEnumerator SpawnProjectilesCoroutine()
    {
        while (health > 0 && !isIdle)
        {
            LaunchProjectile();
            yield return new WaitForSeconds(2f);
        }
    }

    private void LaunchProjectile()
    {
        if (projectilePrefab != null && filamentdobáshely != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, filamentdobáshely.position, filamentdobáshely.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Determine if the player is to the left or right of Veszter
                bool playerToTheRight = GameObject.FindWithTag("Player").transform.position.x > transform.position.x;

                // Flip sprite if necessary
                spriteRenderer.flipX = playerToTheRight;

                float angle = Random.Range(45f, 75f);
                float angleInRadians = angle * Mathf.Deg2Rad;

                float xComponent = playerToTheRight ? Mathf.Cos(angleInRadians) : -Mathf.Cos(angleInRadians);
                float yComponent = Mathf.Sin(angleInRadians);
                Vector2 launchDirection = new Vector2(xComponent, yComponent).normalized;

                rb.AddForce(launchDirection * projectileLaunchForce, ForceMode2D.Impulse);
                Debug.Log($"Projectile launched at angle: {angle} degrees, to the {(playerToTheRight ? "right" : "left")}");
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab or spawn point is missing.");
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        invulnerable = true;

        if (nonTriggerCollider != null)
        {
            nonTriggerCollider.enabled = false; // Disable the non-trigger collider
        }

        float elapsed = 0f;

        while (elapsed < invulnerabilityDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.1f); // Make Veszter semi-transparent
            yield return new WaitForSeconds(blinkInterval / 2);

            spriteRenderer.color = Color.white; // Reset color
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        spriteRenderer.color = Color.white; // Ensure the sprite is fully visible

        if (nonTriggerCollider != null)
        {
            nonTriggerCollider.enabled = true; // Re-enable the non-trigger collider
        }

        invulnerable = false;
    }
}
