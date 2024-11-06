using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Eletek : MonoBehaviour
{
    [Header("Élet")]
    [SerializeField] private float maxElet = 10f;  // Max health
    public float Jelenlegielet { get; private set; }  // Current health

    [HideInInspector][SerializeField] private float iframesduration = 0.2f; // Invincibility duration
    [HideInInspector][SerializeField] private int pirosanvillogas = 1; // Number of invincibility flashes
    private SpriteRenderer spriteRenderer;
    private Animator animator; // Reference to the Animator component

    private AudioSource audioSource;

    private bool isDead = false; // Flag to check if the player is dead
    private bool invulnerable = false; // Flag to check if the player is invulnerable

    private void Awake()
    {
        Jelenlegielet = maxElet;  // Initialize current health to max health
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Connects to the Animator component
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an enemy and the player is alive
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            Sebzodes(1f); // Apply damage (1 as an example, adjust as needed)
        }

        // Allow collision with ground or solid objects even if dead
        if (isDead && (collision.gameObject.CompareTag("Talaj") || collision.gameObject.CompareTag("Solid")))
        {
            return; // Allow these collisions
        }

        // If the player is dead, ignore all other collisions
        if (isDead)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true); // Ignore collision with everything else
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collision is with a "Elet" object (heart)
        if (collision.gameObject.CompareTag("Elet"))
        {
            IncreaseHealth(1f);  // Increase health (adjust as needed)
            Destroy(collision.gameObject);  // Destroy the heart object
        }
    }

    public void Sebzodes(float sebzes)
    {
        if (isDead || invulnerable) return; // If dead or invulnerable, don't take damage

        Jelenlegielet = Mathf.Clamp(Jelenlegielet - sebzes, 0, maxElet);

        if (Jelenlegielet > 0)
        {
            StartCoroutine(Sebezhetetlenseg());
        }
        else
        {
            StartCoroutine(Die()); // Call the Die coroutine to handle death and delay
        }
    }

    private IEnumerator Die()
    {
        isDead = true;

                if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Disable player movement
        GetComponent<Jatekosmozgas>().enabled = false; 

        // Play death animation
        animator.SetBool("Halott", true); 

        // Ignore collisions with enemies
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true); // Ignore collisions with enemies

        // Wait for the animation to play, plus an additional 1-second delay
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 3f);

        // Load Game Over scene
        SceneManager.LoadScene("GameOver");
    }

    private IEnumerator Sebezhetetlenseg()
    {
        invulnerable = true; // Set invulnerable to true
        Physics2D.IgnoreLayerCollision(6, 7, true); // Disable collision with the enemy layer
        for (int i = 0; i < pirosanvillogas; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f); // Flash red
            yield return new WaitForSeconds(iframesduration); // Wait for the duration
            spriteRenderer.color = Color.white; // Reset color
            yield return new WaitForSeconds(iframesduration); // Wait before flashing again
        }
        Physics2D.IgnoreLayerCollision(6, 7, false); // Re-enable collision
        yield return new WaitForSeconds(1f); // Adjust the invulnerability duration
        invulnerable = false; // Reset invulnerability
    }

    private void IncreaseHealth(float amount)
    {
        // Increase health by the specified amount, ensuring it doesn't exceed max health
        Jelenlegielet = Mathf.Clamp(Jelenlegielet + amount, 0, maxElet);
        Debug.Log("Health increased! Current health: " + Jelenlegielet);
    }
}
