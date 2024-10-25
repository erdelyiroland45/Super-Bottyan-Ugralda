using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jatekosmozgas : MonoBehaviour
{
    public float sebesseg = 5.0f;  // A játékos mozgási sebessége
    public float ugrasEro = 7.0f;  // Az ugrás ereje
    private bool aFoldonVan = true;  // Annak nyilvántartása, hogy a játékos a földön van-e

    private Rigidbody2D rb;  // A játékos Rigidbody2D komponense

    // Reference to the SpriteRenderer component
    private SpriteRenderer spriteRenderer;

    // Élet mechanizmus
    public int lives = 3; // A játékos életei

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
        float vizszintesMozgas = Input.GetAxis("Horizontal");  // Jobbra/Balra (A/D vagy bal/jobb nyíl)

        // Mozgási vektor létrehozása (2D-s mozgás, ezért csak X tengely)
        Vector2 mozgasiVektor = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);

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
        if (vizszintesMozgas > 0)
        {
            // Moving right
            spriteRenderer.flipX = true; // Face right
        }
        else if (vizszintesMozgas < 0)
        {
            // Moving left
            spriteRenderer.flipX = false; // Face left
        }
    }

    // Ellenőrzés, hogy a játékos a földön van-e (amikor ütközik valamivel, pl. talaj)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ha a játékos ütközik a talajjal, ismét lehet ugrani
        if (collision.gameObject.CompareTag("Talaj"))
        {
            aFoldonVan = true;
        }

        // Ha a játékos egy ellenséggel ütközik, elveszíti egy életét
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }

    // Életvesztés kezelése
    public void LoseLife()
    {
        lives--;

        Debug.Log("Elvesztettél egy életet! Életek: " + lives);

        if (lives <= 0)
        {
            GameOver();
        }
    }

    // Itt definiálhatod, mi történjen, ha a játékos életei elfogynak
    void GameOver()
    {
        Debug.Log("Game Over!");
        // Itt adhatod meg a Game Over képernyőt vagy újraindíthatod a játékot
        // Például: SceneManager.LoadScene("GameOverScene");
    }
}
