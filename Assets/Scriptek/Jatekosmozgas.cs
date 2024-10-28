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

        // Mozgási vektor létrehozása a sebesség alapján
        Vector2 mozgasiVektor = new Vector2(vizszintesMozgas * sebesseg, rb.velocity.y);
        rb.velocity = mozgasiVektor;

        // Toggle the Mozog parameter based on whether there is horizontal movement
        if (vizszintesMozgas != 0)
        {
            animator.SetBool("Mozog", true);
        }
        else
        {
            animator.SetBool("Mozog", false);
        }

        // Ugrás ellenőrzése (ha W vagy fel nyíl lenyomva és a játékos a földön van)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && aFoldonVan)
        {
            rb.AddForce(Vector2.up * ugrasEro, ForceMode2D.Impulse);
            aFoldonVan = false;
            animator.SetBool("Ugrik", true);
        }
        else if (aFoldonVan)
        {
            animator.SetBool("Ugrik", false);
        }

        // A sprite irányának frissítése a mozgás alapján
        if (vizszintesMozgas > 0)
        {
            spriteRenderer.flipX = true; // Jobbra néz
        }
        else if (vizszintesMozgas < 0)
        {
            spriteRenderer.flipX = false; // Balra néz
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ha a játékos ütközik a talajjal (Ground), ismét lehet ugrani
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ha elhagyja a talajt (Ground), akkor levegőben van
        if (collision.gameObject.CompareTag("Talaj"))
        {
            aFoldonVan = false;
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
