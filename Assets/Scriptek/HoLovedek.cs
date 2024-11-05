using UnityEngine;

public class HoLovedek : MonoBehaviour
{
    public int damageAmount = 1; // Damage dealt to the player
    public float speed = 5f; // Speed of the projectile
    public float destroyHeight = -10f; // Height below which the projectile is destroyed

    private Transform player; // Reference to the player's Transform

    private void Start()
    {
        // Locate the player by tag (ensure your player has the "Player" tag)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found in the scene. Ensure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        // Update direction and move toward the player each frame
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }

        // Destroy the projectile if it falls below a certain height
        if (transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile hits the player
        if (collision.CompareTag("Player"))
        {
            Jatekosmozgas player = collision.GetComponent<Jatekosmozgas>();
            if (player != null)
            {
                player.TakeDamage(damageAmount); // Deal damage to the player
                Destroy(gameObject); // Destroy the projectile upon hitting the player
            }
        }
    }
}
