using UnityEngine;

public class Veertikalisanmozgoplatform : MonoBehaviour
{
    public float moveDistance = 5f;  // Distance the platform moves vertically
    public float moveSpeed = 3f;     // Speed at which the platform moves

    private Vector3 topPoint;         // Calculated highest point
    private Vector3 bottomPoint;      // Calculated lowest point
    private Vector3 nextPosition;     // Next position the platform should move to

    private Transform player;         // Reference to the player

    private void Start()
    {
        // Calculate the top and bottom points dynamically based on the starting position
        topPoint = transform.position + Vector3.up * moveDistance;
        bottomPoint = transform.position;

        nextPosition = topPoint; // Start moving towards the top point
    }

    private void Update()
    {
        // Move the platform vertically
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        // If the platform reaches the next position, reverse direction
        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == topPoint) ? bottomPoint : topPoint;
        }

        // If the player is on the platform, they move with it
        if (player != null)
        {
            // Move the player with the platform if they are standing still
            player.position = new Vector3(player.position.x, transform.position.y, player.position.z);
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
