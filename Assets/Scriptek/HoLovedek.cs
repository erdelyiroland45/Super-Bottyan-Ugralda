using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holovedek : MonoBehaviour
{
    [SerializeField] private float sebzodes = 1f;

    // Desired Z-coordinate value
    private const float fixedZPosition = -1f;

    void Start()
    {
        // Minden szükséges beállítást itt elvégzünk
        // A collider beállítása triggerként biztosítja, hogy a lövedék ne toljon el más objektumokat
    }

    void Update()
    {
        // Set the Z-coordinate of the projectile to always be 1.8
        Vector3 currentPosition = transform.position;
        currentPosition.z = fixedZPosition;  // Lock the Z-coordinate
        transform.position = currentPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Csak a játékossal lép interakcióba
        if (collision.CompareTag("Player"))
        {
            // Játékos sebzése
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Sebzés okozása a játékosnak
                Debug.Log("Sebzést okozott a játékosnak: " + sebzodes);
            }
            Destroy(gameObject); // Lövedék eltüntetése találat után
        }
    }

    private void OnBecameInvisible()
    {
        // Lövedék eltüntetése, ha kikerül a kamera látóteréből
        Destroy(gameObject);
    }
}
