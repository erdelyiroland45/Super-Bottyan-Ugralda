using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eletek : MonoBehaviour
{
    [Header("Elet")]
    [SerializeField] private float maxElet = 10f;  // Max health
    public float Jelenlegielet { get; private set; }  // Current health

    [HideInInspector][SerializeField] private float iframesduration = 0.2f; // Invincibility duration
    [HideInInspector][SerializeField] private int pirosanvillogas = 1; // Number of invincibility flashes
    private SpriteRenderer spriteRenderer;

    private bool isDead = false; // Flag to check if the player is dead
    private bool dead = false; // Flag to check if the player is dead

    private void Awake()
    {
        Jelenlegielet = maxElet;  // Initialize current health to max health
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Sebzodes(1f); // Apply damage (1 as an example, adjust as needed)
        }
    }

    public void Sebzodes(float sebzes)
    {
        Jelenlegielet = Mathf.Clamp(Jelenlegielet - sebzes, 0, maxElet);

        if (Jelenlegielet > 0)
        {
            StartCoroutine(Sebezhetetlenseg());
        }
        else
        {
            if (!isDead && !dead)
            {
                GetComponent<Jatekosmozgas>().enabled = false; // Disable player movement
                isDead = true;
                dead = true;
                // Additional game over logic can go here
            }
        }
    }

    private IEnumerator Sebezhetetlenseg()
    {
        Physics2D.IgnoreLayerCollision(6, 7, true); // Disable collision with the enemy layer
        for (int i = 0; i < pirosanvillogas; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f); // Flash red
            yield return new WaitForSeconds(iframesduration); // Wait for the duration
            spriteRenderer.color = Color.white; // Reset color
            yield return new WaitForSeconds(iframesduration); // Wait before flashing again
        }
        Physics2D.IgnoreLayerCollision(6, 7, false); // Re-enable collision
    }
}
