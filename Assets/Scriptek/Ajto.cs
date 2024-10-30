using UnityEngine;

public class Ajto : MonoBehaviour
{
    [SerializeField] private Sprite nyitottAjtoSprite;  // Nyitott ajtó sprite
    [SerializeField] private Sprite csukottAjtoSprite;  // Csukott ajtó sprite
    private SpriteRenderer spriteRenderer; // Sprite Renderer az ajtóhoz

    private void Start()
    {
        // Inicializáljuk a Sprite Renderert és a csukott ajtó sprite-ját állítjuk be
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = csukottAjtoSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ha a játékos triggereli az ajtót
        if (collision.CompareTag("Player"))
        {
            // Nyitott ajtó sprite-ra váltás
            spriteRenderer.sprite = nyitottAjtoSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Ha a játékos elhagyja a trigger zónát, csukott ajtóra váltás
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.sprite = csukottAjtoSprite;
        }
    }
}
