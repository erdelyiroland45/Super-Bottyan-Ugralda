using UnityEngine;

public class Futodiak : MonoBehaviour
{
    [SerializeField] private float sebzodes; // Damage amount to be applied to the player
    [SerializeField] private float speed = 3.0f; // Speed of the enemy movement
    [SerializeField] private Animator animator; // Reference to the Animator component

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Camera mainCamera; // Reference to the main camera
    private bool isMoving = false; // Flag to check if the enemy is moving

    private void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // Get the main camera

        // Check if the enemy is initially within the camera view
        if (IsWithinCameraView())
        {
            isMoving = true; // Start moving if it is already in view
            animator.SetBool("isMoving", true); // Start the animation
        }
        else
        {
            // Disable the Rigidbody2D to prevent immediate movement
            rb.isKinematic = true; 
        }
    }

    private void Update()
    {
        // If the enemy is not moving, check if it is now within the camera view
        if (!isMoving)
        {
            if (IsWithinCameraView())
            {
                isMoving = true; // Start moving when it enters the camera view
                rb.isKinematic = false; // Re-enable physics
                animator.SetBool("isMoving", true); // Start the animation
            }
            else
            {
                // If it is still out of view, we do nothing
                return;
            }
        }

        // Move the enemy if the flag is set
        if (isMoving)
        {
            MoveEnemy();
        }

        // If the enemy is out of the camera view, destroy it
        if (!IsWithinCameraView())
        {
            Destroy(gameObject);
        }
    }

    private void MoveEnemy()
    {
        // Set the velocity to move left while allowing gravity to affect it
        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }

    private bool IsWithinCameraView()
    {
        // Get the screen bounds
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return (screenPoint.x >= 0 && screenPoint.x <= 1 && 
                screenPoint.y >= 0 && screenPoint.y <= 1);
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
