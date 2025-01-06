using System.Collections;
using UnityEngine;

public class Tabla : MonoBehaviour
{
    public float fallwait = 2f; // Time before the platform starts falling
    private bool isFalling = false; // Tracks whether the platform is falling
    private Rigidbody2D rb; // Reference to the Rigidbody2D component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collided and the platform is not already falling
        if (!isFalling && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall()); // Start the fall coroutine
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true; // Mark the platform as falling
        yield return new WaitForSeconds(fallwait); // Wait for the specified fall delay
        rb.bodyType = RigidbodyType2D.Dynamic; // Enable physics for the platform to start falling
        Destroy(gameObject, 2f); // Destroy the platform after 2 seconds
    }
}
