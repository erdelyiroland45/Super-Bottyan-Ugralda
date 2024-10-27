using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jatekosmozgas : MonoBehaviour
{
    public float sebesseg = 5.0f;  // A játékos mozgási sebessége
    public float ugrasEro = 7.0f;  // Az ugrás ereje
    private bool aFoldonVan = true;  // Annak nyilvántartása, hogy a játékos a földön van-e

    private Rigidbody2D rb;  // A játékos Rigidbody2D komponense
    private SpriteRenderer spriteRenderer; // Referencia a SpriteRendererhez
    private Animator animator; // Referencia az Animatorhoz

    public int lives = 3; // A játékos életei

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Kapcsolódik az Animator komponenshez
    }

    void Update()
    {
        // A vízszintes és függőleges mozgás értékeinek lekérdezése
        float vizszintesMozgas = Input.GetAxis("Horizontal");  // Jobbra/Balra (A/D vagy bal/jobb nyíl)
        float fuggolegesMozgas = Input.GetAxis("Vertical"); // Felfelé/Le (W/S vagy fel/le nyíl)

        // Mozgási vektor létrehozása
        Vector2 mozgasiVektor = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);
        rb.velocity = mozgasiVektor;

        // Animáció paraméter beállítása a mozgási sebesség alapján
        animator.SetFloat("Speed", Mathf.Abs(vizszintesMozgas));

        // Ugrás ellenőrzése (ha W vagy fel nyíl lenyomva és a játékos a földön van)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && aFoldonVan)
        {
            rb.AddForce(new Vector2(0, ugrasEro), ForceMode2D.Impulse);
            aFoldonVan = false;
        }

        // A sprite irányának frissítése a mozgás alapján
        if (vizszintesMozgas > 0)
        {
            spriteRenderer.flipX = false; // Jobbra néz
        }
        else if (vizszintesMozgas < 0)
        {
            spriteRenderer.flipX = true; // Balra néz
        }
    }

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

    // A játék végét kezelő metódus
    void GameOver()
    {
        Debug.Log("Game Over!");
        // Itt jelenítheted meg a Game Over képernyőt vagy újraindíthatod a játékot
    }
}
