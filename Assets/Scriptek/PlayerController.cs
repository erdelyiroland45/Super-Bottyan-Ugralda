using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Mozgás irányának lekérdezése
        float horizontalMovement = Input.GetAxis("Horizontal");

        // Mozgási vektor frissítése
        movementDirection = new Vector2(horizontalMovement * movementSpeed, rb.velocity.y);

        // Karakter irányának beállítása (balra vagy jobbra nézés)
        if (horizontalMovement > 0)
        {
            spriteRenderer.flipX = true; // Jobbra néz
        }
        else if (horizontalMovement < 0)
        {
            spriteRenderer.flipX = false; // Balra néz
        }

        // Ugrás kezelése, ha a játékos a talajon van
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; // Ugrás után levegõben van
        }
    }

    void FixedUpdate()
    {
        // Mozgás alkalmazása
        rb.velocity = new Vector2(movementDirection.x, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ha a karakter talajjal érintkezik, ugrásra készen áll
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Ha elhagyja a talajt, akkor már nincs a földön
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
