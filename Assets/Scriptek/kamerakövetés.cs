using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kamerakovetes : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -1f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform player;

    void LateUpdate()
    {
        // Target position for the camera to follow the player
        Vector3 targetPosition = player.position + offset;
        
        // Smoothly move the camera towards the target position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Lock the camera to pixel-perfect coordinates by rounding to whole numbers
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * 100f) / 100f;  // Multiply/Divide by 100 for 2 decimal precision
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * 100f) / 100f;

        // Apply the adjusted position to the camera
        transform.position = smoothedPosition;
    }
}
