using UnityEngine;
using System.Collections;

public class Bocs : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f;              // Damage dealt to the player
    private int health = 5;                                        // Bocs' health
    private int maxHealth = 5;                                     // Maximum health
    [SerializeField] private int headDamage = 1;                   // Damage taken by Bocs when stomped on
    [SerializeField] private BocsEletbar healthBar;                // Reference to the health bar
    [SerializeField] private GameObject projectilePrefab;          // Prefab for Aktataska projectiles
    [SerializeField] private GameObject egyesProjectilePrefab;     // Prefab for Egyes projectiles
    [SerializeField] private float projectileLaunchForce = 5f;     // Force for launching Aktataska projectiles
    [SerializeField] private float egyesLaunchForce = 3f;          // Force for launching Egyes projectiles
    [SerializeField] private Transform spawnPoint;                 // Spawn point for projectiles
    [SerializeField] private Portalkezelo portalManager;           // Reference to the Portalkezelo (Portal Manager)

    private Coroutine projectileSpawnCoroutine;                    // Reference for the projectile spawning coroutine

    private int stompCount = 0;     
    private bool isVisible = false;                                // Boolean to track visibility
    private int aktataskaCount = 0;                                // Counter for the Aktataska projectiles
    private bool hasMadeMistake = false;                           // Track if a mistake was made in doga

    private bool isIdle = false;  // New flag to indicate Bocs is idle

    public int Health => health;                                   // Public getter for current health
    public int MaxHealth => maxHealth;                             // Public getter for max health

    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Initialize health bar with current health
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

        if (projectileSpawnCoroutine == null && hasMadeMistake && !isIdle) // Only start if a mistake was made and not idle
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

    // The rest of the code remains unchanged...

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isVisible && !isIdle) // Check if Veszter is visible and not idle
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Sebzés okozása a játékosnak
                Debug.Log("Damage dealt to player: " + sebzodes);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isVisible && !isIdle) // Check if Veszter is visible and not idle
        {
            float playerYPosition = collision.transform.position.y;
            float bocsYPosition = transform.position.y;

            if (playerYPosition > bocsYPosition)
            {
                stompCount++; // Növeljük a ugrások számát
                TakeDamage(headDamage); // Sebzés okozása Veszternek
                Debug.Log("Veszter takes head stomp damage");

                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Állítsd be az ugrás erejét szükség szerint
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isVisible || isIdle) return; // Prevent damage if not visible or idle

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Enemy took damage, remaining health: " + health);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Frissítjük az életerő csíkot
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

                        if (portalManager != null)
            {
                portalManager.ActivatePortal();
            }
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

    private IEnumerator SpawnProjectilesCoroutine()
    {
        while (health > 0 && !isIdle)
        {
            if (aktataskaCount < 5)
            {
                LaunchAktataska();
                aktataskaCount++;
                yield return new WaitForSeconds(2f); // Delay between Aktataska projectiles
            }
            else
            {
                // Spawn Egyes projectiles after three Aktataska
                yield return StartCoroutine(LaunchEgyesBurst());
                aktataskaCount = 0; // Reset counter after Egyes burst
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
                Destroy(aktataska); // Destroy if no player found
                return;
            }

            Aktataska aktataskaScript = aktataska.GetComponent<Aktataska>();
            if (aktataskaScript != null)
            {
                aktataskaScript.Initialize(playerTransform, spawnPoint, projectileLaunchForce);
            }
            else
            {
                Destroy(aktataska); // Destroy if no script found
            }
        }
    }

    private IEnumerator LaunchEgyesBurst()
    {
        for (int i = 0; i < 5; i++) // Launch five Egyes projectiles
        {
            LaunchEgyes();
            yield return new WaitForSeconds(0.2f); // Delay between each Egyes projectile
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
                // Random angle between 75 and 120 degrees
                float randomAngle = Random.Range(75f, 120f);
                Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
                
                // Apply force in the random direction
                rb.velocity = direction * egyesLaunchForce;
            }
        }
    }

    // New method to set Bocs idle
    public void GoIdle()
    {
        isIdle = true;
        // Stop any remaining projectiles or damage interactions
        StopCoroutines();
    }
}
