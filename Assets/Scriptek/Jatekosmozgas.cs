using UnityEngine;

public class Jatekosmozgas : MonoBehaviour
{
    public float sebesseg = 5.0f;  // Player movement speed
    public float ugrasEro = 7.0f;  // Jump force
    private bool aFoldonVan = true;  // Tracking if the player is on the ground
    private bool isDead = false;  // Track if the player is dead
    public int health = 3; // Player health

    private Rigidbody2D rb;  // The player's Rigidbody2D component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private Animator animator; // Reference to the Animator

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Connects to the Animator component
        animator.SetBool("Halott", false);
    }

    void Update()
    {
        if (isDead) return; // Stop all input if dead

        float vizszintesMozgas = Input.GetAxis("Horizontal");  // Horizontal input (A/D or Arrow keys)

        // Create movement vector based on speed
        Vector2 mozgasiVektor = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);
        rb.velocity = mozgasiVektor;

        // Check for jump input
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && aFoldonVan)
        {
            Jump();
        }

        // Update sprite direction based on movement
        if (vizszintesMozgas != 0) // Flip sprite regardless of ground status for jumping
        {
            spriteRenderer.flipX = vizszintesMozgas > 0; // Flip sprite to the right when moving right, left when moving left
        }

        // Update animator parameters
        animator.SetBool("Mozog", vizszintesMozgas != 0 && aFoldonVan); // Only moving if on ground
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * ugrasEro, ForceMode2D.Impulse);
        aFoldonVan = false; // Set to airborne after jumping

        // Set the Ugrik parameter to true for the jump animation
        animator.SetBool("Ugrik", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground or jumpable object
        if (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("Solid"))
        {
            aFoldonVan = true; // Allow jumping

            // Set the Ugrik parameter to false to switch back to idle or running
            animator.SetBool("Ugrik", false);
        }

        // Handle collision with enemies
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Call the damage method when colliding with an enemy
            TakeDamage(1);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If leaving the ground or jumpable object, mark as airborne
        if (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("Solid"))
        {
            aFoldonVan = false;
        }
    }

    // Handle game over scenario
    void GameOver()
    {
        if (isDead) return; // Prevent repeated calls to GameOver

        Debug.Log("Game Over!");
        animator.SetBool("Halott", true); // Trigger death animation
        isDead = true; // Set dead flag
        rb.velocity = Vector2.zero; // Stop player movement
    }

    // Method to handle taking damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameOver();
        }
    }
}
