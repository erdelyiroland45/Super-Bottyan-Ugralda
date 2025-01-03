using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool jobbraNez = false; // Az irány kezdéskor balra néz
    private SpriteRenderer spriteRenderer;
    private List<GameObject> aktivKorok = new List<GameObject>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(LovesRutin());
    }

    IEnumerator LovesRutin()
    {
        while (true)
        {
            // Lőjünk az aktuális irányba 10 másodpercig
            yield return StartCoroutine(KorokKilovese());

            // Pihenő 3 másodpercig
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
}
