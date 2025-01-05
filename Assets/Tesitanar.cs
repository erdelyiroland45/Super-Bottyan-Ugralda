using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Az Életbar kezeléséhez szükséges
using UnityEngine.SceneManagement; // A SceneManager névtér importálása

public class Tesitanar : MonoBehaviour
{
    [SerializeField] private GameObject korPrefab; // A kor, amit kilövünk
    [SerializeField] private float lovesIdokoz = 0.5f; // Kilövések közötti időköz
    [SerializeField] private float lovesIdotartam = 10f; // Kilövés időtartama
    [SerializeField] private float pihenoIdotartam = 3f; // Pihenő időtartama
    [SerializeField] private float mozgasTavolsag = 2f; // Mozgás távolsága
    [SerializeField] private float mozgasSebesseg = 2f; // Mozgás sebessége
    [SerializeField] private float korEltunesiIdo = 1f; // Körök eltűnésének időtartama
    [SerializeField] private int maxAktivKorok = 10; // Maximális aktív körök száma
    [SerializeField] private float sebzodes = 1f; // Sebzés a játékosnak
    [SerializeField] private int headDamage = 1; // Sebzés a Tesitanárnak fejre ugráskor
    [SerializeField] private int maxHealth = 5; // Tesitanár maximális életereje
    [SerializeField] private Slider healthBar; // Az Életbar UI elem

    private int currentHealth; // Tesitanár aktuális életereje
    private bool jobbraNez = false; // Az irány kezdéskor balra néz
    private SpriteRenderer spriteRenderer;
    private List<GameObject> aktivKorok = new List<GameObject>();
    private bool sebzheto = false; // A fejre ugrás engedélyezése

    public delegate void TesitanarDeathEvent();
    public static event TesitanarDeathEvent OnTesitanarDeath; // Globális esemény

    public static bool tesitanarMeghalt = false; // Globális változó a Tesitanár halálához

    public int Health
    {
        get { return currentHealth; }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth; // Életerő inicializálása
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        jobbraNez = false; // Kezdésként balra nézzen
        StartCoroutine(LovesRutin());
    }

    IEnumerator LovesRutin()
    {
        while (true)
        {
            // Lőjünk az aktuális irányba 10 másodpercig
            sebzheto = false; // Sebezhetőség kikapcsolása
            spriteRenderer.color = Color.green; // Színváltás cooldown alatt
            yield return StartCoroutine(KorokKilovese());

            // Pihenő 3 másodpercig
            sebzheto = true; // Sebezhetőség bekapcsolása
            spriteRenderer.color = Color.white; // Szín visszaállítása
            yield return new WaitForSeconds(pihenoIdotartam);

            // Mozgás a következő pozícióra
            yield return StartCoroutine(MozgasKovetkezoPoziciora());

            // Irány váltása
            IranyValtas();
        }
    }

    IEnumerator KorokKilovese()
    {
        float elteltIdo = 0f;
        while (elteltIdo < lovesIdotartam)
        {
            KorKilovese();
            yield return new WaitForSeconds(lovesIdokoz);
            elteltIdo += lovesIdokoz;
        }
    }

    void KorKilovese()
    {
        // Ha az aktív körök száma eléri a maximumot, az elsőt eltüntetjük
        if (aktivKorok.Count >= maxAktivKorok)
        {
            StartCoroutine(EltuntetKor(aktivKorok[0]));
            aktivKorok.RemoveAt(0);
        }

        // Létrehozunk egy kört, és sebességet adunk neki
        GameObject kor = Instantiate(korPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = kor.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float irany = jobbraNez ? 1f : -1f; // Az irányt a jobbraNez határozza meg
            rb.velocity = new Vector2(irany * 5f, 0f); // Sebesség beállítása

            // Add angular velocity to simulate rotation
            rb.angularVelocity = -irany * 300f; // Adjust the multiplier for desired rotation speed

            // Ensure the velocity is clamped to prevent compounding bounciness
            StartCoroutine(ClampVelocity(rb, 10f)); // Adjust max velocity as needed
        }

        // Add the kor to the active list
        aktivKorok.Add(kor);
    }

    IEnumerator ClampVelocity(Rigidbody2D rb, float maxVelocity)
    {
        while (rb != null)
        {
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
            yield return null;
        }
    }

    IEnumerator MozgasKovetkezoPoziciora()
    {
        Vector3 celPozicio = transform.position + new Vector3(jobbraNez ? mozgasTavolsag : -mozgasTavolsag, 0f, 0f);
        while (Vector3.Distance(transform.position, celPozicio) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, celPozicio, mozgasSebesseg * Time.deltaTime);
            yield return null;
        }
        transform.position = celPozicio;
    }

    IEnumerator EltuntetKor(GameObject kor)
    {
        // Fade out and destroy the kor
        SpriteRenderer korSprite = kor.GetComponent<SpriteRenderer>();
        if (korSprite != null)
        {
            float elapsedTime = 0f;
            Color originalColor = korSprite.color;
            while (elapsedTime < korEltunesiIdo)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / korEltunesiIdo);
                korSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        Destroy(kor);
    }

    void IranyValtas()
    {
        jobbraNez = !jobbraNez;
        spriteRenderer.flipX = !spriteRenderer.flipX; // Sprite tükrözése horizontálisan
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !sebzheto) // Csak akkor sebződik, ha nem sebezhető
        {
            Eletek playerHealth = collision.GetComponent<Eletek>();
            if (playerHealth != null)
            {
                playerHealth.Sebzodes(sebzodes); // Sebzés okozása a játékosnak
                Debug.Log("Damage dealt to player: " + sebzodes);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && sebzheto) // Csak akkor sebezhető, ha sebzheto = true
        {
            float playerYPosition = collision.transform.position.y;
            float tesitanarYPosition = transform.position.y;

            if (playerYPosition > tesitanarYPosition)
            {
                Eletek playerHealth = collision.gameObject.GetComponent<Eletek>();
                if (playerHealth != null)
                {
                    playerHealth.JumpBoost(); // Opció a játékos visszapattanására
                }
                TakeDamage(headDamage); // Sebzés a Tesitanárnak
                Debug.Log("Tesitanár takes head stomp damage");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Tesitanár took damage: " + damage);
        if (healthBar != null)
        {
            healthBar.value = currentHealth; // Életbar frissítése
        }

        if (currentHealth <= 0)
        {
            Die(); // Tesitanár halála
        }
    }

    void Die()
    {
        Debug.Log("Tesitanár meghalt");

        // Állítsuk true-ra a tesitanarMeghalt változót
        tesitanarMeghalt = true;

        // Esemény indítása, hogy más jelenetben tudják kezelni
        OnTesitanarDeath?.Invoke(); 

        // Scene váltás az A_epulet jelenetre
        SceneManager.LoadScene("A_epulet"); // A_epulet hozzáadása az aktív jelenetekhez

        // Tesitanár megsemmisítése
        Destroy(gameObject); 
    }
}
