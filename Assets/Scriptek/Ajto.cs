using UnityEngine;

public class Ajto : MonoBehaviour
{
    [SerializeField] private Sprite nyitottAjtoSprite;  // Nyitott ajtó sprite
    [SerializeField] private Sprite csukottAjtoSprite;  // Csukott ajtó sprite
    [SerializeField] private GameObject enemyPrefab;    // Az ellenség prefabja
    [SerializeField] private Transform spawnPoint;      // Az ellenség spawn helye
    [SerializeField] private float spawnEsely = 0.25f;  // Spawn esély (25%)

    private SpriteRenderer spriteRenderer; // Sprite Renderer az ajtóhoz
    private bool nyitva = false; // Annak nyilvántartása, hogy az ajtó nyitott-e

    private void Start()
    {
        // Inicializáljuk a Sprite Renderert és beállítjuk a csukott ajtó sprite-ját
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = csukottAjtoSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ha a játékos belép az ajtó triggerébe, kinyitjuk az ajtót
        if (collision.CompareTag("Player") && !nyitva)
        {
            NyisdKiAjtot();
            nyitva = true; // Megakadályozza az ajtó többszöri kinyitását
        }
    }

    private void NyisdKiAjtot()
    {
        // Nyitott ajtó sprite-ra váltás
        spriteRenderer.sprite = nyitottAjtoSprite;

        // Spawn esély ellenõrzése
        if (Random.value < spawnEsely)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        // Debug log az ellenség spawn-olásánál
        Debug.Log("Próbálja spawnolni az ellenséget.");

        if (enemyPrefab != null && spawnPoint != null)
        {
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Ellenség spawnolása sikeres.");
        }
        else
        {
            Debug.LogWarning("Hiányzó enemyPrefab vagy spawnPoint.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Amikor a játékos elhagyja a trigger területet, visszaállítja az ajtót csukott állapotra
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.sprite = csukottAjtoSprite;
            nyitva = false; // Engedélyezi az ajtó újbóli kinyitását, ha újra belép
        }
    }
}
