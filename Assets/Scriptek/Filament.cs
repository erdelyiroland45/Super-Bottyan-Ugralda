using UnityEngine;

public class Filament : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f; // Damage amount to be applied to the player
    [SerializeField] private float speed = 3.0f; // Speed of the enemy movement
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Camera mainCamera; // Reference to the main camera

    private void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // Get the main camera
    }

    private void Update()
    {
        // Move the enemy to the left continuously
        rb.velocity = new Vector2(-speed, rb.velocity.y);

        // Check if the enemy has moved off the left side of the camera view
        if (IsOutOfCameraView())
        {
            Destroy(gameObject); // Despawn the enemy if it's outside the camera view
        }
    }

    private bool IsOutOfCameraView()
    {
        // Get the enemy's position in screen coordinates
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        // Check if the enemy is outside the left side of the screen (viewport x < 0)
        return screenPoint.x < 0;
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
        // Check if the object is tagged as "Talaj" (ground or obstacle)
        else if (collision.CompareTag("Utkozo"))
        {
            // Reverse the horizontal movement direction
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            Debug.Log("Direction reversed after hitting Talaj");
        }
    }
}
