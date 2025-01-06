using UnityEngine;

public class Futodiak : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f; // Damage amount to be applied to the player
    [SerializeField] private float speed = 3.0f; // Speed of the enemy movement
    [SerializeField] private LayerMask tilemapLayer; // LayerMask for detecting walls and ground
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 1f); // Size of the wall check box
    [SerializeField] private Vector2 wallCheckOffset = new Vector2(0.5f, 0f); // Offset for the wall check
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f); // Size of the ground check box
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f); // Offset for the ground check

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private float movementDirection; // Stores the movement direction based on flip state
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private bool isGrounded; // Tracks if the enemy is grounded

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set Rigidbody2D collision detection to Continuous
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Determine initial movement direction based on the Flip X setting in SpriteRenderer
        movementDirection = spriteRenderer.flipX ? 1 : -1;
    }

    private void Update()
    {
        // Perform ground detection
        CheckGround();

        // Only modify the x velocity for movement, let Unity handle gravity
        rb.velocity = new Vector2(speed * movementDirection, rb.velocity.y);

        // Perform wall detection
        if (IsTouchingWall())
        {
            ReverseDirection();
        }
    }

    private void CheckGround()
    {
        // Determine ground check position
        Vector2 groundCheckPosition = (Vector2)transform.position + groundCheckOffset;

        // Perform a BoxCast to detect the ground
        RaycastHit2D groundHit = Physics2D.BoxCast(
            groundCheckPosition,
            groundCheckSize,
            0f,
            Vector2.down, // Ensure we check downward for ground
            0f,
            tilemapLayer
        );

        // Set isGrounded to true if the BoxCast detects the ground
        isGrounded = groundHit.collider != null;

        // If grounded, stop falling, otherwise let gravity do its job
        if (isGrounded && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Reset the vertical velocity
        }
    }

    private bool IsTouchingWall()
    {
        // Adjust wallCheckOffset dynamically based on flipX
        Vector2 wallCheckPosition = (Vector2)transform.position + wallCheckOffset * (spriteRenderer.flipX ? 1 : -1);

        // Perform a BoxCast to detect walls
        RaycastHit2D wallHit = Physics2D.BoxCast(
            wallCheckPosition,
            wallCheckSize,
            0f,
            Vector2.zero,
            0f,
            tilemapLayer
        );

        return wallHit.collider != null; // Return true if the BoxCast detects a wall
    }

    private void ReverseDirection()
    {
        // Reverse the movement direction
        movementDirection *= -1;
        // Flip the sprite horizontally
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (collision.CompareTag("Player"))
        {
            // Get the Eletek component from the player and apply damage
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Call the damage method
                Debug.Log("Damage dealt to player: " + sebzodes); // Log for debugging
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize wall check
        Gizmos.color = Color.red;
        Vector2 wallCheckPosition = (Vector2)transform.position + wallCheckOffset * (spriteRenderer != null && spriteRenderer.flipX ? 1 : -1);
        Gizmos.DrawWireCube(wallCheckPosition, wallCheckSize);

        // Visualize ground check
        Gizmos.color = Color.green;
        Vector2 groundCheckPosition = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireCube(groundCheckPosition, groundCheckSize);
    }
}
