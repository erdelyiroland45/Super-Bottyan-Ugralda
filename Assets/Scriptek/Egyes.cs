using UnityEngine;

public class Egyes : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f; // Damage amount to apply to the player
    [SerializeField] private Collider2D damageCollider; // The trigger collider for damaging the player
    [SerializeField] private Collider2D mainCollider;   // The non-trigger collider for collision detection with Talaj
    private Camera mainCamera;                          // Reference to the main camera

    private void Start()
    {
        mainCamera = Camera.main; // Get the main camera

        // Ignore all collisions for the main collider except those with Talaj
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D collider in allColliders)
        {
            if (collider.CompareTag("Talaj") == false && collider != mainCollider)
            {
                Physics2D.IgnoreCollision(collider, mainCollider);
            }
        }
    }

    private void Update()
    {
        // Optional: Add any movement logic here

        // Despawn the projectile if it goes off-screen
        if (IsOutOfCameraView())
        {
            Destroy(gameObject);
        }
    }

    private bool IsOutOfCameraView()
    {
        // Get the projectile's position in screen coordinates
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        // Check if the projectile is outside the left side of the screen (viewport x < 0)
        return screenPoint.x < 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the trigger collider has collided with the Player
        if (collision.CompareTag("Player"))
        {
            // Get the Eletek component from the player and apply damage
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Apply damage to the player
                Debug.Log("Damage dealt to player: " + sebzodes); // Log for debugging
            }
            // Optionally destroy the projectile after hitting the player
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is tagged as "Talaj"
        if (collision.gameObject.CompareTag("Talaj"))
        {
            Destroy(gameObject); // Destroy the projectile upon hitting the ground
        }
    }
}
