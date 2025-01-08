using UnityEngine;
using UnityEngine.SceneManagement;

public class Jatekosmozgas : MonoBehaviour
{
    [SerializeField] private float sebesseg = 5.0f;  // Player movement speed
    [SerializeField] private float ugrasEro = 7.0f;  // Jump force
    private bool isGrounded = false;  // Track if the player is grounded
    private bool isTouchingWall = false;  // Track if the player is touching a wall

    private Rigidbody2D rb;  // The player's Rigidbody2D component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private Animator animator; // Reference to the Animator

    [SerializeField] private LayerMask tilemapLayer;  // LayerMask for the tilemap

    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);  // Size of the ground check box
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 1f);    // Size of the wall check box
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f); // Offset for the ground check
    [SerializeField] private Vector2 wallCheckOffset = new Vector2(0.5f, 0f);   // Offset for the wall check

    private Eletek eletek; // Reference to the Eletek script
    private Transform platform;  // Reference to the platform the player is standing on

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        eletek = GetComponent<Eletek>(); // Get the Eletek component

        animator.SetBool("Halott", false);

        // Set the initial position as a checkpoint if no checkpoint is set
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetInitialCheckpoint(transform.position, SceneManager.GetActiveScene().name);
        }

        // Respawn at the last checkpoint if available
        if (CheckpointManager.Instance != null && CheckpointManager.Instance.LastSceneName == SceneManager.GetActiveScene().name)
        {
            transform.position = CheckpointManager.Instance.LastCheckpointPosition;
        }
        else
        {
            // Update the checkpoint to the player's initial position in the new level
            if (CheckpointManager.Instance != null)
            {
                CheckpointManager.Instance.SetCheckpoint(transform.position, SceneManager.GetActiveScene().name);
            }
        }
    }

    void Update()
    {
        if (eletek.isDead)  // Check if the player is dead
        {
            rb.velocity = Vector2.zero; // Stop the player's movement
            return; // Prevent any further input or actions if the player is dead
        }

        // Perform ground and wall checks
        CheckGroundAndWall();

        // Horizontal movement
        float vizszintesMozgas = Input.GetAxis("Horizontal");

        // If the player is on a platform, move with the platform
        if (platform != null)
        {
            // The platform's movement will move the player horizontally
            rb.velocity = new Vector2(vizszintesMozgas * sebesseg + platform.GetComponent<Rigidbody2D>().velocity.x, rb.velocity.y);
        }
        else
        {
            // Normal movement without platform
            rb.velocity = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);
        }

        // Update sprite direction
        if (vizszintesMozgas != 0)
        {
            spriteRenderer.flipX = vizszintesMozgas > 0;
        }

        // Jump: Only allow jumping when grounded
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            Jump();
        }

        // Update animator parameters
        animator.SetBool("Mozog", vizszintesMozgas != 0 && isGrounded);

        // Update jump animation state
        animator.SetBool("Ugrik", !isGrounded);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * ugrasEro, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void CheckGroundAndWall()
    {
        // Ground detection
        RaycastHit2D groundHit = Physics2D.BoxCast(
            (Vector2)transform.position + groundCheckOffset,
            groundCheckSize,
            0f,
            Vector2.down,
            0f,
            tilemapLayer
        );

        isGrounded = groundHit.collider != null; // Set isGrounded to true if the BoxCast detects the ground
    }

    // This method is called when the player touches the platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platform = collision.transform; // Set the platform as the player's parent
            transform.SetParent(platform);  // Parent the player to the platform

            // Optional: you could adjust the player's position relative to the platform
            // to make sure they stay on the platform properly.
        }
    }

    // This method is called when the player leaves the platform
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platform = null; // Clear the reference to the platform
            transform.SetParent(null);  // Remove the parent-child relationship
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);
    }
}