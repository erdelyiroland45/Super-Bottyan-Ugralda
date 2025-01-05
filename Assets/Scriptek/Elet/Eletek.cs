using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Eletek : MonoBehaviour
{
    [Header("Élet")]
    [SerializeField] private float maxElet = 10f;
    public float Jelenlegielet { get; private set; }

    [HideInInspector][SerializeField] private float iframesduration = 0.2f;
    [HideInInspector][SerializeField] private int pirosanvillogas = 1;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private AudioSource audioSource;
    private Rigidbody2D rb;

    public bool isDead { get; private set; } = false;
    private bool invulnerable = false;

    [Header("Jump Boost")]
    [SerializeField] private float jumpBoostForce = 10f; // Adjustable force for JumpBoost

    private void Awake()
    {
        Jelenlegielet = maxElet;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on the player object.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            Sebzodes(1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Deadzone"))
        {
            StartCoroutine(Die());
        }
        else if (collision.gameObject.CompareTag("Elet"))
        {
            IncreaseHealth(1f);
            Destroy(collision.gameObject);
        }
    }

    public void Sebzodes(float sebzes)
    {
        if (isDead || invulnerable) return;

        Jelenlegielet = Mathf.Clamp(Jelenlegielet - sebzes, 0, maxElet);

        if (Jelenlegielet > 0)
        {
            StartCoroutine(Sebezhetetlenseg());
        }
        else
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        if (isDead) yield break;

        isDead = true;

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        animator.SetBool("Halott", true);

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 3f);

        SceneManager.LoadScene("GameOver");
    }

    private IEnumerator Sebezhetetlenseg()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(6, 7, true);

        for (int i = 0; i < pirosanvillogas; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iframesduration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iframesduration);
        }

        Physics2D.IgnoreLayerCollision(6, 7, false);
        yield return new WaitForSeconds(1f);
        invulnerable = false;
    }

    private void IncreaseHealth(float amount)
    {
        Jelenlegielet = Mathf.Clamp(Jelenlegielet + amount, 0, maxElet);
        Debug.Log("Health increased! Current health: " + Jelenlegielet);
    }

    public void JumpBoost()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity
            rb.AddForce(Vector2.up * jumpBoostForce, ForceMode2D.Impulse);
            Debug.Log("Player launched upwards with JumpBoost.");
        }
    }
}
