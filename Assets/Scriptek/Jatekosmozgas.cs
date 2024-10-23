using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jatekosmozgas : MonoBehaviour
{
    public float sebesseg = 7.0f;          // A játékos normál mozgási sebessége
    public float ugrasEro = 7.0f;          // Az ugrás ereje
    private bool aFoldonVan = true;         // Annak nyilvántartása, hogy a játékos a földön van-e

    private Rigidbody2D rb;                 // A játékos Rigidbody2D komponense
    private SpriteRenderer spriteRenderer;   // A sprite rendereléséért felelős komponens

    // Jeges felületen lévő csúszási paraméterek
    public bool jegesFeluletenVan = false;  // Nyilvántartja, hogy jégen vagyunk-e
    public float jegesSebesseg = 7.0f;      // Csúszási sebesség a jégen
    public float gyorsulas = 20.0f;         // A gyorsulás mértéke
    public float lassulas = 2.0f;           // A lassulás mértéke (amikor nincs input)

    private float currentSpeed = 0.0f;      // Jelenlegi sebesség a vízszintes tengelyen

    void Start()
    {
        // Kapjuk meg a Rigidbody2D komponenst
        rb = GetComponent<Rigidbody2D>();
        
        // Get the SpriteRenderer component to change the sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // A vízszintes mozgás értékének lekérdezése (A/D vagy bal/jobb nyíl)
        float vizszintesInput = Input.GetAxis("Horizontal");  // Jobbra/Balra (A/D vagy bal/jobb nyíl)

        // Ellenőrizzük, hogy a játékos jégen van-e
        if (jegesFeluletenVan)
        {
            // Ha jégen vagyunk, a gyorsulás/lassulás lassabb
            if (vizszintesInput != 0)
            {
                // Fokozatos gyorsulás jégen, ha nyomva tartjuk a gombot
                currentSpeed = Mathf.MoveTowards(currentSpeed, vizszintesInput * jegesSebesseg, gyorsulas * Time.deltaTime);
            }
            else
            {
                // Lassú lassulás, ha nincs mozgás input
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, lassulas * Time.deltaTime);
            }
        }
        else
        {
            // Normál talajon, azonnal követi az inputot
            currentSpeed = vizszintesInput * sebesseg;
        }

        // Mozgási vektor létrehozása (2D-s mozgás, ezért csak X tengely)
        Vector2 mozgasiVektor = new Vector2(currentSpeed, rb.velocity.y);

        // A játékos pozíciójának frissítése (sebesség beállítása)
        rb.velocity = mozgasiVektor;

        // Ugrás ellenőrzése (ha W vagy fel nyíl lenyomva és a játékos a földön van)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && aFoldonVan)
        {
            // Ugrás végrehajtása (vertikális erő hozzáadása a Rigidbody2D-hoz)
            rb.AddForce(new Vector2(0, ugrasEro), ForceMode2D.Impulse);

            // Most már a levegőben van a játékos
            aFoldonVan = false;
        }

        // Flip the sprite based on movement direction
        if (currentSpeed > 0)
        {
            // Jobbra mozog
            spriteRenderer.flipX = false; // Arc jobbra
        }
        else if (currentSpeed < 0)
        {
            // Balra mozog
            spriteRenderer.flipX = true; // Arc balra
        }
    }

    // Ellenőrzés, hogy a játékos a földön van-e (amikor ütközik valamivel, pl. talaj)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug üzenet, hogy lásd, mikor ütközik a játékos
        Debug.Log("Collision with: " + collision.gameObject.name);

        // Ha a játékos ütközik a talajjal, ismét lehet ugrani
        if (collision.gameObject.CompareTag("Talaj"))
        {
            aFoldonVan = true;
            jegesFeluletenVan = false;  // Nem vagyunk a jégen
        }

        // Ha a játékos ütközik a jéggel (Ice)
        if (collision.gameObject.CompareTag("Jég"))
        {
            jegesFeluletenVan = true;  // Jégen vagyunk
            aFoldonVan = true;          // Ezen a ponton állítsuk be, hogy a játékos a földön van
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ha a játékos elhagyja a jeget (Ice)
        if (collision.gameObject.CompareTag("Jég"))
        {
            jegesFeluletenVan = false;  // Már nem vagyunk a jégen
        }
    }
}
