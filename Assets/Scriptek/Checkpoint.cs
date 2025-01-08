using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CheckpointManager.Instance?.SetCheckpoint(transform.position, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            Debug.Log("Checkpoint set at: " + transform.position + " in scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}