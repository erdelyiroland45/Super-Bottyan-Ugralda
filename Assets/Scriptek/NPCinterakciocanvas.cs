using UnityEngine;

public class Bolt : MonoBehaviour
{
    public static bool BoltActive = false; // Tracks if the shop is active
    [SerializeField] private GameObject shopUI; // Shop UI Canvas element
    [SerializeField] private TextMesh interactText; // Regular TextMesh for interaction prompt

    private bool isPlayerInRange = false; // Tracks if the player is near the shop

    private void Start()
    {
        shopUI.SetActive(false); // Initially hide the shop UI
        interactText.gameObject.SetActive(false); // Hide the interaction text initially
    }

    private void Update()
    {
        // Toggle shop UI if player presses "E" while in range
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }

        // Close shop if "Escape" is pressed while shop is open
        if (BoltActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void ToggleShop()
    {
        if (BoltActive)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    private void OpenShop()
    {
        shopUI.SetActive(true); // Show shop UI
        Time.timeScale = 0f; // Pause the game
        BoltActive = true;
        interactText.gameObject.SetActive(false); // Hide interaction prompt when shop is open
    }

    private void CloseShop()
    {
        shopUI.SetActive(false); // Hide shop UI
        Time.timeScale = 1f; // Resume the game
        BoltActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // Player entered the shop zone
            interactText.gameObject.SetActive(true); // Show interaction text
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // Player left the shop zone
            interactText.gameObject.SetActive(false); // Hide interaction text

            // Close the shop if the player leaves the zone while it’s open
            if (BoltActive) 
                CloseShop();
        }
    }
}
