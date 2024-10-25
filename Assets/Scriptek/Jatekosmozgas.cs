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

    void Start()
    {
        // Kapjuk meg a Rigidbody2D komponenst
        rb = GetComponent<Rigidbody2D>();
        
        // Get the SpriteRenderer component to change the sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
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
            spriteRenderer.flipX = false; // Face right
        }
        else if (vizszintesMozgas < 0)
        {
            // Moving left
            spriteRenderer.flipX = true; // Face left
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
    }
}
