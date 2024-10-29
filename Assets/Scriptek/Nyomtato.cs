using UnityEngine;

public class Nyomtato : MonoBehaviour
{
    [SerializeField] private float sebzodes; // Damage amount to be applied to the player

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that collided is tagged as "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamage(collision.gameObject); // Deal damage when the player collides with it
        }
    }

    private void DealDamage(GameObject player)
    {
        // Get the Eletek component from the player and apply damage
        Eletek playerHealth = player.GetComponent<Eletek>();
        if (playerHealth != null)
        {
            playerHealth.Sebzodes(sebzodes); // Call the damage method
            Debug.Log("Damage dealt to player: " + sebzodes); // Log for debugging
        }
    }
}
