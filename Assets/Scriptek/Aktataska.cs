using UnityEngine;

public class Aktataska : MonoBehaviour
{
    private Transform target; // Target (Player)
    private Transform origin; // Starting point (Bocs)
    private float speed = 0.1f; // Speed of the Aktataska
    private bool returning = false; // Tracks whether the Aktataska is returning
    private float launchDistance = 8f; // Maximum distance to travel towards the player
    private Vector3 launchDirection; // Direction in which Aktataska was launched
    private float traveledDistance = 0f; // Distance traveled from origin

    // Initialization function
    public void Initialize(Transform target, Transform origin, float speed)
    {
        this.target = target;
        this.origin = origin;
        this.speed = speed;

        // Calculate launch direction towards the target (Player)
        launchDirection = (target.position - origin.position).normalized; // Normalized direction vector
    }

    private void Update()
    {
        if (returning)
        {
            // Move back towards the origin point
            transform.position = Vector2.MoveTowards(transform.position, origin.position, speed * Time.deltaTime);

            // Check if it reached the origin
            if (Vector2.Distance(transform.position, origin.position) < 0.1f)
            {
                Destroy(gameObject); // Destroy Aktataska on return
            }
        }
        else
        {
            // Move in the launch direction
            transform.position += launchDirection * speed * Time.deltaTime;
            traveledDistance += speed * Time.deltaTime; // Increment traveled distance

            // Check if it reached the launch distance, then start returning
            if (traveledDistance >= launchDistance)
            {
                returning = true; // Start returning to origin
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile collides with the player
        if (collision.CompareTag("Player"))
        {
            Eletek playerHealth = collision.GetComponent<Eletek>(); // Get the player's health component
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(1); // Deal damage to the player (change 1 to the desired damage value)
                Debug.Log("Player hit by Aktataska. Damage dealt: 1");
            }

            Destroy(gameObject); // Destroy the Aktataska after dealing damage
        }
    }
}
