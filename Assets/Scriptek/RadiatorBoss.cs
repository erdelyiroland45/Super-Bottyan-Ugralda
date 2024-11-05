using UnityEngine;

public class RadiatorBoss : MonoBehaviour
{
    public GameObject snowProjectilePrefab; // Prefab a hó lövedékhez
    public Transform firePoint; // A pont, ahonnan a lövés indul
    public float fireRate = 1.5f; // Lövés gyakorisága másodpercenként
    public float projectileSpeed = 5f; // Lövedék sebessége

    private float nextFireTime = 0f;

    private void Update()
    {
        if (Time.time >= nextFireTime)
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
        rb.velocity = firePoint.right * projectileSpeed;
    }
}
