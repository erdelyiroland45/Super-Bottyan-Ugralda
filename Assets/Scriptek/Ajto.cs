using UnityEngine;

public class Ajto : MonoBehaviour
{
    [SerializeField] private Sprite nyitottAjtoSprite;  // Open door sprite
    [SerializeField] private Sprite csukottAjtoSprite;  // Closed door sprite
    [SerializeField] private GameObject enemyPrefab1;   // First enemy prefab to spawn
    [SerializeField] private GameObject enemyPrefab2;   // Second enemy prefab to spawn
    [SerializeField] private Transform spawnPoint;      // Spawn location for the enemy
    [SerializeField] private float spawnEsely = 0.25f;  // Spawn chance (25%)

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
            // 50-50 chance to spawn either enemyPrefab1 or enemyPrefab2
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        // Check if both enemy prefabs and the spawn point are set
        if (spawnPoint != null)
        {
            GameObject chosenEnemy = Random.value < 0.5f ? enemyPrefab1 : enemyPrefab2;
            if (chosenEnemy != null)
            {
                Instantiate(chosenEnemy, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("Enemy spawned successfully: " + chosenEnemy.name);
            }
            else
            {
                Debug.LogWarning("One of the enemy prefabs is missing.");
            }
        }
        else
        {
            Debug.LogWarning("Missing spawnPoint.");
        }
    }
}
