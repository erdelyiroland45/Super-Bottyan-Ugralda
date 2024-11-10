using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ajto : MonoBehaviour
{
    [SerializeField] private Sprite newDoorSprite; // New sprite for the door
    private SpriteRenderer doorSpriteRenderer; // Reference to the door's SpriteRenderer

    // Start is called before the first frame update
    void Start()
    {
        // Get the SpriteRenderer component attached to the door object
        doorSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // You can add any other logic here if needed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (collision.CompareTag("Player"))
        {
            // Change the sprite of the door
            if (doorSpriteRenderer != null && newDoorSprite != null)
            {
                doorSpriteRenderer.sprite = newDoorSprite; // Set the new sprite for the door
                Debug.Log("Door sprite changed!");
            }
        }
    }
}
