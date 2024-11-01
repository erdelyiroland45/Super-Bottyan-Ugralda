using UnityEngine;

public class Futodiak : MonoBehaviour
{
    [SerializeField] private float sebzodes; // Damage amount to be applied to the player
    [SerializeField] private float speed = 3.0f; // Speed of the enemy movement
    [SerializeField] private Animator animator; // Reference to the Animator component

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Camera mainCamera; // Reference to the main camera

    private void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // Get the main camera

        // Enable physics and start the animation
        rb.isKinematic = false; 
        animator.SetBool("isMoving", true);
    }

    private void Update()
    {
        // Move the enemy
        MoveEnemy();

        // Check if the enemy is out of the left side of the camera
        if (IsOutOfCameraView())
        {
            Destroy(gameObject); // Destroy the enemy if it's out of the camera view
        }
    }

    private void MoveEnemy()
    {
        // Set the velocity to move left while allowing gravity to affect it
        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }

    private bool IsOutOfCameraView()
    {
        // Get the screen bounds
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0; // Returns true if the enemy is outside the left side of the camera
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (collision.CompareTag("Player"))
        {
            // Get the Eletek component from the player and apply damage
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Call the damage method
                Debug.Log("Damage dealt to player: " + sebzodes); // Log for debugging
            }
        }
    }
}
