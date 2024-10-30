using UnityEngine;

public class Jatekosmozgas : MonoBehaviour
{
    public float sebesseg = 5.0f;  // Player movement speed
    public float ugrasEro = 7.0f;  // Jump force
    private bool aFoldonVan = true;  // Tracking if the player is on the ground

    private Rigidbody2D rb;  // The player's Rigidbody2D component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private Animator animator; // Reference to the Animator

    public int lives = 3; // Player's lives

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Connects to the Animator component
    }

    void Update()
    {
        float vizszintesMozgas = Input.GetAxis("Horizontal");  // Horizontal input (A/D or Arrow keys)

        // Create movement vector based on speed
        Vector2 mozgasiVektor = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);
        rb.velocity = mozgasiVektor;

        // Toggle the Mozog parameter based on whether there is horizontal movement
        animator.SetBool("Mozog", vizszintesMozgas != 0);

        // Jump check (allow jump if player is on the ground or jumpable object)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && aFoldonVan)
        {
            rb.AddForce(Vector2.up * ugrasEro, ForceMode2D.Impulse);
            aFoldonVan = false; // Set to airborne after jumping
        }

        // Update sprite direction based on movement
        spriteRenderer.flipX = vizszintesMozgas > 0; // Flip sprite based on direction
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground or jumpable object
        if (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("Solid"))
        {
            aFoldonVan = true; // Allow jumping
        }

        // Handle collision with enemies
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
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

    // Handle losing life
    public void LoseLife()
    {
        lives--;
        Debug.Log("Elvesztettél egy életet! Életek: " + lives);
        if (lives <= 0)
        {
            GameOver();
        }
    }

    // Handle game over scenario
    void GameOver()
    {
        Debug.Log("Game Over!");
        // Show Game Over screen or restart the game here
    }
}
