using UnityEngine;
using UnityEngine.SceneManagement; // Hozzáadva a jelenetváltáshoz

public class Palyaajto : MonoBehaviour
{
    [SerializeField] private TextMesh interactText; // Regular TextMesh for interaction prompt

    private bool isDoorInRange = false; // Tracks if the player is near the door

    private void Start()
    {
        interactText.gameObject.SetActive(false); // Hide the interaction text initially
    }

    private void Update()
    {
        // Handle door interaction
        if (isDoorInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }

    private void TryOpenDoor()
    {
            SceneManager.LoadScene("Tornaterem"); // Load the new scene
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isDoorInRange = true; // Player entered the door zone
            interactText.text = "Nyomd E";
            interactText.gameObject.SetActive(true); // Show interaction text
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isDoorInRange = false; // Player left the door zone
            interactText.gameObject.SetActive(false); // Hide interaction text
        }
    }
}
