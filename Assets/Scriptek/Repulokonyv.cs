using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulokonyv : MonoBehaviour
{
    public float speed = 5f; // Adjustable speed of the enemy
    public float detectionRadius = 10f; // Radius for detecting the player
    private Transform player;
    private Camera mainCamera;
    private bool isFollowing = false; // Tracks if the enemy is currently following the player

    void Start()
    {
        // Find the player object (assuming it has the "Player" tag)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        
        // Get the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                isFollowing = true;
            }
            else
            {
                isFollowing = false;
            }

            if (isFollowing && IsVisibleOnCamera())
            {
                FollowPlayer();
            }
        }
    }

    bool IsVisibleOnCamera()
    {
        if (mainCamera == null) return false;

        // Convert the enemy's position to viewport space
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Check if the object is within the camera's viewport
        return viewportPosition.z > 0 && 
               viewportPosition.x > 0 && viewportPosition.x < 1 && 
               viewportPosition.y > 0 && viewportPosition.y < 1;
    }

    void FollowPlayer()
    {
        // Move towards a point slightly lower than the player
        Vector3 targetPosition = player.position;
        targetPosition.y -= 1f; // Adjust this value to set how much lower the target should be

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Access the player's Eletek script and apply damage
            Eletek playerHealth = collision.gameObject.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(1f); // Deal 1 damage to the player
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection radius in the scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
