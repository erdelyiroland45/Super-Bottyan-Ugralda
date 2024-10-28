using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eletek : MonoBehaviour
{
    [SerializeField] private float maxElet = 10f;  // Max health
    public float Jelenlegielet { get; private set; }  // Current health

    private void Awake()
    {
        Jelenlegielet = maxElet;  // Initialize current health to max health
    }

    public void Sebzodes(float sebzes)
    {
        Jelenlegielet = Mathf.Clamp(Jelenlegielet - sebzes, 0, maxElet);
        Debug.Log($"Current Health: {Jelenlegielet}"); // Log the current health

        if (Jelenlegielet > 0)
        {
            // Player is still alive
            Debug.Log("Player is alive");
        }
        else
        {
            // Player is dead, add death handling logic here
            Debug.Log("Player is dead");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Sebzodes(1);  // Reduces health by 1 each time E is pressed
        }
    }
}
