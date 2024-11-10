using UnityEngine;

public class Futodiak : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f; // Damage amount to be applied to the player
    [SerializeField] private float speed = 3.0f; // Speed of the enemy movement
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Camera mainCamera; // Reference to the main camera
    private float movementDirection; // Stores the movement direction based on flip state
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    private void Start()
    {
        // Get the Rigidbody2D and SpriteRenderer components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main; // Get the main camera

        // Determine initial movement direction based on the Flip X setting in SpriteRenderer
        movementDirection = spriteRenderer.flipX ? 1 : -1;
    }

    private void Update()
    {
        // Move the enemy based on the determined direction
        rb.velocity = new Vector2(speed * movementDirection, rb.velocity.y);

        // Check if the enemy has moved off the left or right side of the camera view
        if (IsOutOfCameraView())
        {
            Destroy(gameObject); // Despawn the enemy if it's outside the camera view
        }
    }

    private bool IsOutOfCameraView()
    {
        // Get the enemy's position in screen coordinates
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        // Check if the enemy is outside the screen (viewport x < 0 or x > 1)
        return screenPoint.x < 0 || screenPoint.x > 1;
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
