using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    [Range(10, 100)][SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime = 3.0f;
    public int damage = 25;  // Default damage value

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = GetComponentInParent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Bullet>().damage = this.damage;  // Assign damage to bullet
        bullet.GetComponent<Rigidbody>().velocity = playerCamera.transform.forward * bulletSpeed;
        Destroy(bullet, bulletLifeTime);
    }
}
