using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
        [SerializeField] private float sebzodes = 1f;
    private Collider2D projectileCollider; // Store the projectile's collider


    void Start()
    {
        projectileCollider = GetComponent<Collider2D>();
        
        // Ignore all collisions
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D collider in allColliders)
        {
            if (collider.CompareTag("Talaj") == false) // Ignore all except the ground
            {
                Physics2D.IgnoreCollision(collider, projectileCollider);
            }
        }
    }

    void Update()
    {
        // Add any projectile movement logic here if needed
    }

        private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Sebzés okozása a játékosnak
                Debug.Log("Damage dealt to player: " + sebzodes);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is tagged as "Talaj"
        if (collision.gameObject.CompareTag("Talaj"))
        {
            // Destroy the projectile upon hitting the ground
            Destroy(gameObject);
        }
    }
}
