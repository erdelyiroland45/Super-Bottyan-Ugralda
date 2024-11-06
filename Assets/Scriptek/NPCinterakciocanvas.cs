using UnityEngine;

public class Bolt : MonoBehaviour
{
    public static bool BoltActive = false; // Annak nyilv�ntart�sa, hogy a bolt akt�v-e
    [SerializeField] private GameObject shopUI; // A v�s�rl�i fel�let Canvas eleme

    private bool isPlayerInRange = false; // Jelzi, ha a j�t�kos a bolt k�zel�ben van

    private void Start()
    {
        shopUI.SetActive(false); // Kezdetben l�thatatlan a boltfel�let
    }

    private void Update()
    {
        // Ha a j�t�kos a bolt k�zel�ben van, �s megnyomja az "E" gombot
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }

        // Ha a bolt akt�v, �s megnyomja az "ESC" gombot, bez�rjuk a boltot
        if (BoltActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void ToggleShop()
    {
        // A boltfel�let l�that�s�g�nak kapcsol�sa
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
        shopUI.SetActive(true); // Boltfel�let megjelen�t�se
        Time.timeScale = 0f; // J�t�k meg�ll�t�sa
        BoltActive = true;
    }

    private void CloseShop()
    {
        shopUI.SetActive(false); // Boltfel�let elrejt�se
        Time.timeScale = 1f; // J�t�k folytat�sa
        BoltActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // A j�t�kos a bolt trigger ter�let�n bel�l van
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // A j�t�kos elhagyta a bolt trigger ter�let�t
            if (BoltActive) CloseShop(); // Bez�rjuk a boltot, ha a j�t�kos kil�p a ter�letb�l
        }
    }
}
