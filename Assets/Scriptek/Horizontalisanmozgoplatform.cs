using UnityEngine;

public class Horizontalisanmozgoplatform : MonoBehaviour
{
    public float moveDistance = 5f;  // Distance the platform moves (5 units)
    public float moveSpeed = 2f;     // Speed at which the platform moves

    private Vector3 pointA;          // Starting position of the platform
    private Vector3 pointB;          // Target position of the platform
    private Vector3 nextPosition;    // Next position the platform should move to

    private Transform player;        // Reference to the player

    private void Start()
    {
        // Set initial positions for the movement
        pointA = transform.position;
        pointB = transform.position + Vector3.right * moveDistance;
        nextPosition = pointB; // Start moving towards point B
    }

    private void Update()
    {
        // Move the platform
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        // If the platform reaches the next position, reverse direction
        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA) ? pointB : pointA;
        }

        // If the player is on the platform, they move with it
        if (player != null)
        {
            // Move the player with the platform if they are standing still
            player.position = new Vector3(transform.position.x, player.position.y, player.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.transform; // Set the player reference
            collision.gameObject.transform.parent = transform; // Make the platform the parent of the player
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = null; // Clear the reference to the player
            collision.gameObject.transform.parent = null; // Remove the parent-child relationship
        }
    }
}
