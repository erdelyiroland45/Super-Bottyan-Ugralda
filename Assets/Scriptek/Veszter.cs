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
    [SerializeField] private GameObject keyPrefab; // Kulcs prefab
    [SerializeField] private Transform keySpawnPoint; // A kulcs spawn pontja

    private Coroutine filamentSpawnCoroutine;      // Hivatkozás a filamentek spawnolásáért felelős coroutine-ra
    private Coroutine projectileSpawnCoroutine;    // Hivatkozás a projektilok spawnolásáért felelős coroutine-ra
    private int stompCount = 0;                    // A fejre ugrások számlálója
    private bool isVisible = false;                // Boolean a láthatóság nyomon követésére

    public int Health => health;                   // Publikus getter a jelenlegi életerőhöz
    public int MaxHealth => maxHealth;             // Publikus getter a maximális életerőhöz

    private void Start()
    {
        filamentSpawnCoroutine = null;
        projectileSpawnCoroutine = null; // Initial null for coroutine tracking
        
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
        isVisible = false;
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false); // Hide health bar when not visible
        }
        
        StopCoroutines();
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

            // Kulcs spawnolása
            SpawnKey();
        }
    }

    private void SpawnKey()
    {
        if (keyPrefab != null)
        {
            Vector3 spawnPosition = keySpawnPoint != null ? keySpawnPoint.position : transform.position;
            Instantiate(keyPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Key spawned successfully.");
        }
        else
        {
            Debug.LogWarning("Key prefab is missing.");
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
                float angle = Random.Range(45f, 75f);
                float angleInRadians = angle * Mathf.Deg2Rad;

                float xComponent = -Mathf.Cos(angleInRadians);
                float yComponent = Mathf.Sin(angleInRadians);
                Vector2 launchDirection = new Vector2(xComponent, yComponent).normalized;

                rb.AddForce(launchDirection * projectileLaunchForce, ForceMode2D.Impulse);
                Debug.Log($"Projectile launched left at angle: {angle} degrees.");
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab or spawn point is missing.");
        }
    }
}
