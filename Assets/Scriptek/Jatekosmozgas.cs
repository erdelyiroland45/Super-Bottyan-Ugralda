using UnityEngine;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        eletek = GetComponent<Eletek>(); // Get the Eletek component

        animator.SetBool("Halott", false);
    }

    void Update()
    {
        if (eletek.isDead)  // Check if the player is dead
        {
            rb.velocity = Vector2.zero; // Stop the player's movement
            return; // Prevent any further input or actions if the player is dead
        }

        // Horizontal movement
        float vizszintesMozgas = Input.GetAxis("Horizontal");
        Vector2 mozgasiVektor = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);
        rb.velocity = mozgasiVektor;

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

        // Perform ground and wall checks
        CheckGroundAndWall();
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * ugrasEro, ForceMode2D.Impulse);
        isGrounded = false;
        animator.SetBool("Ugrik", true);
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

        // Wall detection
        Vector2 wallCheckDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        Vector2 wallCheckPosition = (Vector2)transform.position + wallCheckOffset * (spriteRenderer.flipX ? 1 : -1);
        RaycastHit2D wallHit = Physics2D.BoxCast(
            wallCheckPosition,
            wallCheckSize,
            0f,
            wallCheckDirection,
            0f,
            tilemapLayer
        );

        isTouchingWall = wallHit.collider != null; // Set isTouchingWall to true if the BoxCast detects a wall

        // Update jump animation state
        animator.SetBool("Ugrik", !isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);

        // Visualize wall check
        Gizmos.color = Color.blue;
        Vector2 wallCheckPosition = (Vector2)transform.position + wallCheckOffset * (spriteRenderer != null && spriteRenderer.flipX ? 1 : -1);
        Gizmos.DrawWireCube(wallCheckPosition, wallCheckSize);
    }
}
