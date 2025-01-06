using UnityEngine;
using System.Collections;

public class Bocs : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f;              // Damage dealt to the player
    private int health = 5;                                    // Bocs' health
    private int maxHealth = 5;                                 // Maximum health
    [SerializeField] private int headDamage = 1;               // Damage taken by Bocs when stomped on
    [SerializeField] private BocsEletbar healthBar;            // Reference to the health bar
    [SerializeField] private GameObject projectilePrefab;      // Prefab for Aktataska projectiles
    [SerializeField] private GameObject egyesProjectilePrefab; // Prefab for Egyes projectiles
    [SerializeField] private float projectileLaunchForce = 5f; // Force for launching Aktataska projectiles
    [SerializeField] private float egyesLaunchForce = 3f;      // Force for launching Egyes projectiles
    [SerializeField] private Transform spawnPoint;             // Spawn point for projectiles
    [SerializeField] private Portalkezelo portalManager;       // Reference to the Portalkezelo (Portal Manager)
    [SerializeField] private float invulnerabilityDuration = 3f; // Duration of invulnerability
    [SerializeField] private float blinkInterval = 0.2f;       // Interval for blinking effect

    private SpriteRenderer spriteRenderer;
    private Collider2D nonTriggerCollider;                    // Reference to the non-trigger collider
    private Coroutine projectileSpawnCoroutine;
    private int stompCount = 0;
    private bool isVisible = false;                           // Boolean to track visibility
    private int aktataskaCount = 0;                           // Counter for the Aktataska projectiles
    private bool hasMadeMistake = false;                      // Track if a mistake was made in doga
    private bool invulnerable = false;                        // Tracks if Bocs is invulnerable
    private bool isIdle = false;                              // New flag to indicate Bocs is idle

    public int Health => health;                              // Public getter for current health
    public int MaxHealth => maxHealth;                        // Public getter for max health

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on Bocs.");
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
            Debug.LogError("Non-trigger collider not found on Bocs.");
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
            healthBar.gameObject.SetActive(false); // Hide health bar initially
        }
    }

    private void OnEnable()
    {
        doga.OnMistakeMade += EnableAttack; // Subscribe to event
    }

    private void OnDisable()
    {
        doga.OnMistakeMade -= EnableAttack; // Unsubscribe from event
    }

    private void EnableAttack()
    {
        hasMadeMistake = true;
        if (isVisible && !isIdle && projectileSpawnCoroutine == null)
        {
            projectileSpawnCoroutine = StartCoroutine(SpawnProjectilesCoroutine());
        }
    }

    private void OnBecameVisible()
    {
        isVisible = true;
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true); // Show health bar when visible
        }

        if (projectileSpawnCoroutine == null && hasMadeMistake && !isIdle)
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
        if (collision.CompareTag("Player") && isVisible && !isIdle)
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes);
                Debug.Log("Damage dealt to player: " + sebzodes);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isVisible && !isIdle)
        {
            float playerYPosition = collision.transform.position.y;
            float bocsYPosition = transform.position.y;

            if (playerYPosition > bocsYPosition)
            {
                stompCount++;
                TakeDamage(headDamage);
                Debug.Log("Bocs takes head stomp damage");

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
        if (!isVisible || isIdle || invulnerable) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Enemy took damage, remaining health: " + health);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
        }

        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Bocs destroyed");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false);
            }
            StopCoroutines();

            if (portalManager != null)
            {
                portalManager.ActivatePortal();
            }
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
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
            spriteRenderer.color = new Color(1, 1, 1, 0.1f); // Make Bocs semi-transparent
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

    private void StopCoroutines()
    {
        if (projectileSpawnCoroutine != null)
        {
            StopCoroutine(projectileSpawnCoroutine);
            projectileSpawnCoroutine = null;
        }
    }

    private IEnumerator SpawnProjectilesCoroutine()
    {
        while (health > 0 && !isIdle)
        {
            if (aktataskaCount < 5)
            {
                LaunchAktataska();
                aktataskaCount++;
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return StartCoroutine(LaunchEgyesBurst());
                aktataskaCount = 0;
            }
        }
    }

    private void LaunchAktataska()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            GameObject aktataska = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

            Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null)
            {
                Destroy(aktataska);
                return;
            }

            Aktataska aktataskaScript = aktataska.GetComponent<Aktataska>();
            if (aktataskaScript != null)
            {
                aktataskaScript.Initialize(playerTransform, spawnPoint, projectileLaunchForce);
            }
            else
            {
                Destroy(aktataska);
            }
        }
    }

    private IEnumerator LaunchEgyesBurst()
    {
        for (int i = 0; i < 5; i++)
        {
            LaunchEgyes();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void LaunchEgyes()
    {
        if (egyesProjectilePrefab != null && spawnPoint != null)
        {
            GameObject egyesProjectile = Instantiate(egyesProjectilePrefab, spawnPoint.position, spawnPoint.rotation);

            Rigidbody2D rb = egyesProjectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float randomAngle = Random.Range(75f, 120f);
                Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
                rb.velocity = direction * egyesLaunchForce;
            }
        }
    }

    public void GoIdle()
    {
        isIdle = true;
        StopCoroutines();
    }
}
