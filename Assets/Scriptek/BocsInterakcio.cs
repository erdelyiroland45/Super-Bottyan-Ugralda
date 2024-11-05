using UnityEngine;

public class BocsInterakcio : MonoBehaviour
{
    [SerializeField] private GameObject bocsUI; // Reference to the first UI to show
    [SerializeField] private GameObject doga1Canvas; // Reference to the Doga1 Canvas
    [SerializeField] private Collider2D triggerZone; // Reference to the trigger zone for the interaction

    private bool hasInteracted = false; // Flag to track if the player has interacted with the trigger zone

    private void Start()
    {
        // Log to check if triggerZone is assigned properly
        if (triggerZone == null)
        {
            triggerZone = GetComponent<Collider2D>(); // Automatically get the collider if not assigned
            Debug.Log("Trigger Zone is automatically assigned.");
        }

        // Ensure the UI is initially hidden
        if (bocsUI != null)
        {
            bocsUI.SetActive(false); // Hide the UI at the start
            Debug.Log("Bocs UI is hidden at the start.");
        }
        else
        {
            Debug.LogError("Bocs UI is not assigned in the inspector!");
        }

        // Ensure the Doga1 Canvas is initially hidden
        if (doga1Canvas != null)
        {
            doga1Canvas.SetActive(false); // Hide the Doga1 Canvas at the start
            Debug.Log("Doga1 Canvas is hidden at the start.");
        }
        else
        {
            Debug.LogError("Doga1 Canvas is not assigned in the inspector!");
        }
    }

    // This will be called when the player enters the trigger zone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasInteracted) // Ensure the colliding object is the player and check if interaction has not occurred
        {
            Debug.Log("Player entered the trigger zone.");
            hasInteracted = true; // Set the flag to true so the UI won't show again
            bocsUI.SetActive(true); // Show UI
            ShowBocsUI();
        }
    }

    // Show the Bocs UI and freeze the game
    public void ShowBocsUI()
    {
        if (bocsUI != null)
        {
            Debug.Log("Showing Bocs UI.");
            bocsUI.SetActive(true); // Show UI
            Time.timeScale = 0f; // Freeze the game
        }
        else
        {
            Debug.LogError("Bocs UI is not assigned in the inspector!");
        }
    }

    // This will check for input to unfreeze the game
    private void Update()
    {
        // Check if the Bocs UI is visible by checking if it is active in the scene
        if (bocsUI != null && bocsUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) // Check for E or Esc key press
            {
                Debug.Log("Player pressed E or Escape, hiding Bocs UI and unfreezing the game.");
                HideBocsUI(); // Hide the UI
                ShowDoga1Canvas(); // Show the Doga1 Canvas
            }
        }
    }

    // Hide the Bocs UI
    public void HideBocsUI()
    {
        if (bocsUI != null)
        {
            bocsUI.SetActive(false);
            Debug.Log("Hiding Bocs UI");
        }
    }

    // Show the Doga1 Canvas
    private void ShowDoga1Canvas()
    {
        if (doga1Canvas != null)
        {
            doga1Canvas.SetActive(true); // Show the Doga1 Canvas
            Debug.Log("Showing Doga1 Canvas.");
        }
        else
        {
            Debug.LogError("Doga1 Canvas is not assigned in the inspector!");
        }
    }
}
