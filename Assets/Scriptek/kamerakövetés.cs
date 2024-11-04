using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kamerakovetes : MonoBehaviour
{
    // Offset for initial camera position
    private Vector3 offset = new Vector3(2f, 2f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform player;

    // Define horizontal and vertical thresholds
    private float horizontalThreshold = -20f; // The player must move 2 units to the right of the camera to make it follow
    private float verticalThreshold = 1f;   // The player must be 1 unit above/below the camera center for it to follow

    void LateUpdate()
    {
        if (player == null) return; // Ensure the player is assigned

        // Get the current camera position
        Vector3 targetPosition = transform.position;

        // Horizontal tracking: Only update X position if the player is beyond the horizontal threshold
        if (player.position.x > transform.position.x + horizontalThreshold)
        {
            targetPosition.x = player.position.x + offset.x;
        }

        // Vertical tracking: Only update Y position if the player is beyond the vertical threshold
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);
        if (verticalDistance > verticalThreshold)
        {
            // Move camera halfway towards the player’s position on the Y-axis for a smooth follow
            targetPosition.y = Mathf.Lerp(transform.position.y, player.position.y + offset.y, 0.5f);
        }

        // Smoothly move the camera towards the target position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Optional: Lock the camera to pixel-perfect coordinates by rounding to whole numbers (2 decimal precision)
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * 100f) / 100f;
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * 100f) / 100f;
        smoothedPosition.z = Mathf.Round(smoothedPosition.z * 100f) / 100f;

        // Apply the adjusted position to the camera
        transform.position = smoothedPosition;
    }
}
