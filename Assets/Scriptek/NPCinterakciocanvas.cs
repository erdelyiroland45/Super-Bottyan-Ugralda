using UnityEngine;

public class Bolt : MonoBehaviour
{
    public static bool BoltActive = false; // Annak nyilvántartása, hogy a bolt aktív-e
    [SerializeField] private GameObject shopUI; // A vásárlói felület Canvas eleme

    private bool isPlayerInRange = false; // Jelzi, ha a játékos a bolt közelében van

    private void Start()
    {
        shopUI.SetActive(false); // Kezdetben láthatatlan a boltfelület
    }

    private void Update()
    {
        // Ha a játékos a bolt közelében van, és megnyomja az "E" gombot
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }

        // Ha a bolt aktív, és megnyomja az "ESC" gombot, bezárjuk a boltot
        if (BoltActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void ToggleShop()
    {
        // A boltfelület láthatóságának kapcsolása
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
        shopUI.SetActive(true); // Boltfelület megjelenítése
        Time.timeScale = 0f; // Játék megállítása
        BoltActive = true;
    }

    private void CloseShop()
    {
        shopUI.SetActive(false); // Boltfelület elrejtése
        Time.timeScale = 1f; // Játék folytatása
        BoltActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // A játékos a bolt trigger területén belül van
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // A játékos elhagyta a bolt trigger területét
            if (BoltActive) CloseShop(); // Bezárjuk a boltot, ha a játékos kilép a területbõl
        }
    }
}
