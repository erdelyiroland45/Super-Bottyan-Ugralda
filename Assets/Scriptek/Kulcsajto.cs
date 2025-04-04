using UnityEngine;
using UnityEngine.SceneManagement; // Hozzáadva a jelenetváltáshoz

public class Kulcsajto : MonoBehaviour
{
    [SerializeField] private TextMesh interactText; // Regular TextMesh for interaction prompt

    private bool isDoorInRange = false; // Tracks if the player is near the door

    public bool hasKey = false; // Tracks if the player has a key

    private void Start()
    {
        interactText.gameObject.SetActive(false); // Hide the interaction text initially
    }

    private void Update()
    {
        // Handle door interaction
        if (isDoorInRange && Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            TryOpenDoor();
        }
    }

    private void TryOpenDoor()
    {
        if (hasKey)
        {
            SceneManager.LoadScene("B_Epulet"); // Load the new scene
        }
        else
        {
            Debug.Log("The door is locked. You need a key to open it.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isDoorInRange = true; // Player entered the door zone
            interactText.text = "Press E to open the door";
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
