using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kamerakovetes : MonoBehaviour
{
    // Adjust the offset to position the camera slightly in front of the player and above
    private Vector3 offset = new Vector3(2f, 2f, -10f);  // Increase Y value for a higher camera position
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform player;

    void LateUpdate()
    {
        if (player == null) return; // Ensure the player is assigned

        // Calculate target position based on player position and offset
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera towards the target position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Lock the camera to pixel-perfect coordinates by rounding to whole numbers
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * 100f) / 100f;  // 2 decimal precision
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * 100f) / 100f;
        smoothedPosition.z = Mathf.Round(smoothedPosition.z * 100f) / 100f;

        // Apply the adjusted position to the camera
        transform.position = smoothedPosition;
    }
}
