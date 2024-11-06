using UnityEngine;
using UnityEngine.UI;

public class IvettEletbar : MonoBehaviour
{
    [SerializeField] private Ivett ivett;      // Reference to the Veszter (mini-boss) component
    [SerializeField] private Image osszeselet;     // Reference to the overall health bar (background)
    [SerializeField] private Image jelenlegielet;  // Reference to the current health bar (foreground)

    private void Start()
    {
        // Initialize the health bar
        UpdateHealthBar(); // Set to max at the start
    }

    private void Update()
    {
        // Update the health bar each frame
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (ivett != null && jelenlegielet != null)
        {
            // Set fill amount based on current health divided by 10 (to match the 10 hearts)
            jelenlegielet.fillAmount = (float)ivett.Health / 10f;

            // Ensure fill amount doesn't exceed 1 (100%)
            if (jelenlegielet.fillAmount > 1f)
            {
                jelenlegielet.fillAmount = 1f; // Clamp to 1 if it exceeds
            }
        }
    }
}
