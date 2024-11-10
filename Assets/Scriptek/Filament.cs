using UnityEngine;

public class Filament : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f; // Damage amount to be applied to the player
    [SerializeField] private float speed = 3.0f; // Speed of the enemy movement
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Camera mainCamera; // Reference to the main camera
    private int direction = -1; // Default movement direction to the left (-1)
    private bool hasReversed = false; // Flag to track if direction has been reversed in a collision
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        mainCamera = Camera.main; // Get the main camera

        // Ensure the sprite is initially facing left
        spriteRenderer.flipX = false;
    }

    private void Update()
    {
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);

        // Check if the enemy has moved off the left side of the camera view
        if (IsOutOfCameraView())
        {
            Destroy(gameObject); // Despawn the enemy if it's outside the camera view
            return;
        }
    }

    private bool IsOutOfCameraView()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes);
                Debug.Log("Damage dealt to player: " + sebzodes);
            }
        }
        else if (collision.CompareTag("Fal"))
        {
            // Only reverse direction once per collision
            if (!hasReversed)
            {
                ReverseDirection();
                hasReversed = true; // Set the flag to true after reversing
            }
        }
        else
        {
            Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset the flag when the enemy exits collision with "Fal"
        if (collision.CompareTag("Fal"))
        {
            hasReversed = false;
        }
    }

    private void ReverseDirection()
    {
        rb.velocity = Vector2.zero;
        direction = -direction; // Toggle direction between -1 and 1

        // Flip the sprite based on the new direction
        spriteRenderer.flipX = direction > 0;

        // Log the direction each time it reverses
        Debug.Log("Direction reversed to: " + direction);
    }
}
