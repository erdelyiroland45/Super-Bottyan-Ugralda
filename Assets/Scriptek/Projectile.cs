using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
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
