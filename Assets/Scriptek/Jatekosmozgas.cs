using UnityEngine;
using UnityEngine.SceneManagement;

public class Jatekosmozgas : MonoBehaviour
{
    [SerializeField] private float sebesseg = 5.0f;  // Player movement speed
    [SerializeField] private float ugrasEro = 7.0f;  // Jump force
    [SerializeField] private int health = 3; // Player health
    
    private bool isGrounded = false;  // Track if the player is grounded
    private bool isTouchingWall = false;  // Track if the player is touching a wall
    private bool isDead = false;  // Track if the player is dead

    private Rigidbody2D rb;  // The player's Rigidbody2D component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private Animator animator; // Reference to the Animator

    private Eletek eletekScript; // Reference to Eletek script

    [SerializeField] private BoxCollider2D talajDetectionCollider; // Reference to the TalajDetection collider (child object)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Connects to the Animator component
        animator.SetBool("Halott", false);

        // Get the Eletek script component to check if the player is dead
        eletekScript = GetComponent<Eletek>();

        // If talajDetectionCollider isn't assigned through the Inspector, try to automatically get it
        if (talajDetectionCollider == null)
        {
            talajDetectionCollider = transform.Find("TalajDetection").GetComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        // If the player is dead, stop all input and movement
        if (eletekScript.isDead) 
        {
            isDead = true;
            rb.velocity = Vector2.zero;  // Stop the player from moving
            return;
        }

        // Horizontal movement
        float vizszintesMozgas = Input.GetAxis("Horizontal");  // Horizontal input (A/D or Arrow keys)
        
        // Create movement vector based on speed
        Vector2 mozgasiVektor = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);
        rb.velocity = mozgasiVektor;

        // Update sprite direction based on movement
        if (vizszintesMozgas != 0)
        {
            spriteRenderer.flipX = vizszintesMozgas > 0; // Flip sprite to the right when moving right, left when moving left
        }

        // Check for jump input (W or Up Arrow keys)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            Jump();
        }

        // Update animator parameters
        animator.SetBool("Mozog", vizszintesMozgas != 0 && isGrounded); // Only moving if on ground
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * ugrasEro, ForceMode2D.Impulse);
        isGrounded = false; // Set to airborne after jumping

        // Set the Ugrik parameter to true for the jump animation
        animator.SetBool("Ugrik", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Flags to track whether we're grounded and touching a wall in the same frame
        bool isCollidingWithGround = false;
        bool isCollidingWithWall = false;

        // Iterate through all the contacts in the collision
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Check for a ground collision (normal pointing upwards) with either "Talaj" or "TalajFalKombo"
            if (contact.normal.y > 0.5f && (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("TalajFalKombo")))
            {
                isCollidingWithGround = true; // Player is grounded
            }
            // Check for a wall collision (normal pointing horizontally) with either "Talaj" or "TalajFalKombo"
            else if (Mathf.Abs(contact.normal.x) > 0.5f && (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("TalajFalKombo")))
            {
                isCollidingWithWall = true; // Player is touching a wall
            }
        }

        // Update grounded status if colliding with the ground, but ignore wall collisions for jumping logic
        if (isCollidingWithGround)
        {
            isGrounded = true; // Player is grounded
            animator.SetBool("Ugrik", false); // Reset jump animation state
        }

        // If colliding with a wall, we don't update grounded status for jumping
        if (isCollidingWithWall)
        {
            isTouchingWall = true; // Player is touching a wall
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Iterate through all the contacts in the collision
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // If we were grounded and now leaving the ground, mark as airborne
            if (contact.normal.y > 0.5f && (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("TalajFalKombo")))
            {
                isGrounded = false; // Player is no longer grounded
            }

            // If we were touching a wall, update the flag
            if (Mathf.Abs(contact.normal.x) > 0.5f && (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("TalajFalKombo")))
            {
                isTouchingWall = false; // Player is no longer touching a wall
            }
        }
    }
}
