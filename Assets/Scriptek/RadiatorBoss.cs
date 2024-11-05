using UnityEngine;

public class RadiatorBoss : MonoBehaviour
{
    public GameObject snowProjectilePrefab; // Prefab a hó lövedékhez
    public Transform firePoint; // A pont, ahonnan a lövés indul
    public float fireRate = 1.5f; // Lövés gyakorisága másodpercenként
    public float projectileSpeed = 5f; // Lövedék sebessége

    private float nextFireTime = 0f;
    private bool isVisible = false; // Állapot, hogy a RadiatorBoss látható-e a képernyőn

    private void Update()
    {
        // Csak akkor lő, ha látható a képernyőn és elérkezett az idő
        if (isVisible && Time.time >= nextFireTime)
        {
            ShootSnow();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootSnow()
    {
        // Létrehozzuk a hó lövedéket és beállítjuk a sebességét
        GameObject snowProjectile = Instantiate(snowProjectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = snowProjectile.GetComponent<Rigidbody2D>();

        // A lövedék balra mozog a firePoint bal iránya mentén
        rb.velocity = firePoint.right * -projectileSpeed;
    }

    // Ez a metódus hívódik meg, amikor a RadiatorBoss láthatóvá válik a képernyőn
    private void OnBecameVisible()
    {
        isVisible = true;
    }

    // Ez a metódus hívódik meg, amikor a RadiatorBoss kikerül a képernyő látóteréből
    private void OnBecameInvisible()
    {
        isVisible = false;
    }
}
