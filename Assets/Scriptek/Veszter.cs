using UnityEngine;

public class Veszter : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f;  // A játékosnak okozott sebzés
    private int health = 5;                        // Az ellenség életereje
    private int maxHealth = 5;                     // Maximális életerő
    [SerializeField] private int headDamage = 1;   // Sebzés, amit Veszter kap, amikor a játékos a fejére ugrik
    [SerializeField] private VeszterEletbar healthBar; // Hivatkozás az életerő csíkra
    [SerializeField] private GameObject[] filamentPrefabs; // Filament prefabok tömbje
    [SerializeField] private Transform spawnPoint; // A filamentek spawn helye
    [SerializeField] private Transform filamentdobáshely; // Új spawn pont a projektilokhoz
    [SerializeField] private float spawnDelay = 3f; // Időköz a filamentek spawnolása között
    [SerializeField] private GameObject projectilePrefab; // Prefab a projektilhoz
    [SerializeField] private float projectileLaunchForce = 5f; // Erő, amivel a projektilt kilövik
    [SerializeField] private float projectileInterval = 2f; // Időköz a projektilok kilövése között

    private Coroutine filamentSpawnCoroutine;      // Hivatkozás a filamentek spawnolásáért felelős coroutine-ra
    private Coroutine projectileSpawnCoroutine;     // Hivatkozás a projektilok spawnolásáért felelős coroutine-ra
    private int stompCount = 0;                    // A fejre ugrások számlálója
    private bool isVisible = false;                 // Boolean a láthatóság nyomon követésére

    public int Health => health;                   // Publikus getter a jelenlegi életerőhöz
    public int MaxHealth => maxHealth;             // Publikus getter a maximális életerőhöz

    private void Start()
    {
        // Nincs kezdeti spawnolás
        filamentSpawnCoroutine = null;
        projectileSpawnCoroutine = null; // Initial null for coroutine tracking
    }

    private void Update()
    {
        // Only allow actions if Veszter is visible
        if (isVisible)
        {
            // Any logic that should only happen when visible can go here
        }
    }

    private void OnBecameVisible()
    {
        // When Veszter becomes visible, start the coroutines
        isVisible = true;
        if (filamentPrefabs.Length > 0 && filamentSpawnCoroutine == null)
        {
            filamentSpawnCoroutine = StartCoroutine(SpawnFilamentCoroutine());
        }
        if (projectileSpawnCoroutine == null)
        {
            projectileSpawnCoroutine = StartCoroutine(SpawnProjectilesCoroutine());
        }
    }

    private void OnBecameInvisible()
    {
        // When Veszter becomes invisible, stop the coroutines
        isVisible = false;
        if (filamentSpawnCoroutine != null)
        {
            StopCoroutine(filamentSpawnCoroutine);
            filamentSpawnCoroutine = null;
        }
        if (projectileSpawnCoroutine != null)
        {
            StopCoroutine(projectileSpawnCoroutine);
            projectileSpawnCoroutine = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isVisible) // Check if Veszter is visible
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
        if (collision.gameObject.CompareTag("Player") && isVisible) // Check if Veszter is visible
        {
            float playerYPosition = collision.transform.position.y;
            float veszterYPosition = transform.position.y;

            if (playerYPosition > veszterYPosition)
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
        if (!isVisible) return; // Prevent damage if not visible

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Enemy took damage, remaining health: " + health);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(); // Frissítjük az életerő csíkot
        }

        if (health <= 0)
        {
            Destroy(gameObject); // Megsemmisítjük az ellenséget
            Debug.Log("Enemy destroyed");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false); // Elrejtjük az életerő csíkot
            }
            StopCoroutines();
        }
    }

    private void StopCoroutines()
    {
        if (filamentSpawnCoroutine != null)
        {
            StopCoroutine(filamentSpawnCoroutine);
            filamentSpawnCoroutine = null;
        }
        if (projectileSpawnCoroutine != null)
        {
            StopCoroutine(projectileSpawnCoroutine);
            projectileSpawnCoroutine = null;
        }
    }

    private System.Collections.IEnumerator SpawnFilamentCoroutine()
    {
        while (health > 0)
        {
            SpawnFilament();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnFilament()
    {
        if (spawnPoint != null)
        {
            GameObject chosenFilament = filamentPrefabs[Random.Range(0, filamentPrefabs.Length)];
            Instantiate(chosenFilament, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Filament spawned successfully.");
        }
        else
        {
            Debug.LogWarning("Spawn point is missing.");
        }
    }

    private System.Collections.IEnumerator SpawnProjectilesCoroutine()
    {
        while (health > 0)
        {
            LaunchProjectile();
            yield return new WaitForSeconds(projectileInterval);
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
                float randomXOffset = Random.Range(-0.5f, -1.0f);
                float randomYOffset = Random.Range(0.5f, 1.0f);

                Vector2 launchDirection = new Vector2(randomXOffset, randomYOffset).normalized;
                rb.AddForce(launchDirection * projectileLaunchForce, ForceMode2D.Impulse);
            }
            Debug.Log("Projectile launched with random direction.");
        }
        else
        {
            Debug.LogWarning("Projectile prefab or spawn point is missing.");
        }
    }
}