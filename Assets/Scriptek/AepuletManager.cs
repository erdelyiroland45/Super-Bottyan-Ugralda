using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Listák használatához

public class A_epuletManager : MonoBehaviour
{
    [SerializeField] private GameObject barrier;

    void Update()
    {
        // Figyeljük a globális változót, hogy a Tesitanár meghalt-e
        if (Tesitanar.tesitanarMeghalt)
        {
            // Ha a Tesitanár meghalt, logoljuk, hogy aktiválódott a manager
            Debug.Log("A_epuletManager aktiválódott, Tesitanár meghalt.");

            // Ha a Tesitanár meghalt, eltüntetjük az összes AlloDiak tag-jű objektumot és a barrier-t
            StartCoroutine(TorlesKeses());
            Tesitanar.tesitanarMeghalt = false; // Reseteljük a változót, hogy elkerüljük többszöri törlést
        }
    }

    IEnumerator TorlesKeses()
    {
        yield return new WaitForSeconds(1f); // Késleltetés
    
        bool sikerultEltuntetni = false; // Flag a sikeresség ellenőrzésére

        // Keresünk minden objektumot, ami az "AlloDiak" tag-et viseli
        GameObject[] alloDiakok = GameObject.FindGameObjectsWithTag("AlloDiak");

        // Töröljük az összes AlloDiak tag-jű objektumot
        foreach (GameObject diak in alloDiakok)
        {
            if (diak != null)
            {
                // Ha a diák szülő objektum, töröljük az összes child objektumot is
                Transform[] children = diak.GetComponentsInChildren<Transform>(); // Lekérjük az összes child-ot

                foreach (Transform child in children)
                {
                    if (child != diak.transform) // Ne töröljük magát a szülőt
                    {
                        DestroyImmediate(child.gameObject); // Töröljük a child objektumot
                        Debug.Log("Egy child diák eltüntetve!");
                        sikerultEltuntetni = true;
                    }
                }

                // Töröljük a diák szülőt is
                DestroyImmediate(diak); 
                Debug.Log("Szülő diák eltüntetve!");
                sikerultEltuntetni = true;
            }
            else
            {
                Debug.LogWarning("Egy diák nem található, nem sikerült eltüntetni.");
            }
        }

        // Ha barrier is létezik, azt is eltüntetjük
        if (barrier != null)
        {
            Destroy(barrier);
            Debug.Log("Barrier eltüntetve!");
            sikerultEltuntetni = true; // A törlés sikerült
        }
        else
        {
            Debug.LogWarning("Barrier nem található, nem sikerült eltüntetni.");
        }

        if (!sikerultEltuntetni)
        {
            Debug.LogError("Nem sikerült eltüntetni egy vagy több objektumot!");
        }
    }
}
