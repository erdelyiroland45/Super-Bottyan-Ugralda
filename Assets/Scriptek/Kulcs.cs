using UnityEngine;

public class Kulcs : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Kulcsajto doorScript = FindObjectOfType<Kulcsajto>(); // Megkeressük a Kulcsajto scriptet
            if (doorScript != null)
            {
                doorScript.hasKey = true; // Jelöljük, hogy a játékos megszerezte a kulcsot
                Debug.Log("Key picked up!");
            }

            Destroy(gameObject); // Eltávolítjuk a kulcsot a pályáról
        }
    }
}
