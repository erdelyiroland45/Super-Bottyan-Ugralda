using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Add this for UI components

public class Eletbar : MonoBehaviour
{
    [SerializeField] private Eletek jatekoseletei;  // Reference to the player health data
    [SerializeField] private Image osszeselet;       // Reference to the overall health bar
    [SerializeField] private Image jelenlegielet;    // Reference to the current health bar

    private void Start()
    {
        // Set the current health amount based on the player's current health
        jelenlegielet.fillAmount = jatekoseletei.Jelenlegielet / 10f;  // Use 10f to indicate floating-point division
    }

    private void Update()
    {
        // Update the current health amount each frame
        jelenlegielet.fillAmount = jatekoseletei.Jelenlegielet / 10f;  // Use 10f to avoid integer division
    }
}
