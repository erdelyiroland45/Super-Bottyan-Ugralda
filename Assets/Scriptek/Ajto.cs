using UnityEngine;

public class Ajto : MonoBehaviour
{
    [SerializeField] private Sprite nyitottAjtoSprite;  // Open door sprite
    [SerializeField] private Sprite csukottAjtoSprite;  // Closed door sprite
    [SerializeField] private GameObject enemyPrefab;     // Enemy prefab to spawn
    [SerializeField] private Transform spawnPoint;       // Spawn location for the enemy
    [SerializeField] private float spawnEsely = 0.25f;   // Spawn chance (25%)

    private SpriteRenderer spriteRenderer; // Sprite Renderer for the door
    private bool nyitva = false; // Tracks whether the door is open

    private void Start()
    {
        // Initialize Sprite Renderer and set the closed door sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = csukottAjtoSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player enters the door trigger and the door is not already open
        if (collision.CompareTag("Player") && !nyitva)
        {
            NyisdKiAjtot();
            nyitva = true; // Prevents the door from opening multiple times
        }
    }

    private void NyisdKiAjtot()
    {
        // Change to the open door sprite
        spriteRenderer.sprite = nyitottAjtoSprite;

        // Check spawn chance
        if (Random.value < spawnEsely)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        // Debug log for enemy spawning
        Debug.Log("Attempting to spawn enemy.");

        if (enemyPrefab != null && spawnPoint != null)
        {
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Enemy spawned successfully.");
        }
        else
        {
            Debug.LogWarning("Missing enemyPrefab or spawnPoint.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Removed sprite reset to keep the door open
    }
}
