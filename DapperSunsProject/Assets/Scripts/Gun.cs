using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    [Range(10, 100)][SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifeTime = 3.0f;
    [SerializeField] public int damage = 25;  // Default damage value

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
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(playerCamera.transform.forward * bulletSpeed, damage);
        Destroy(bullet, bulletLifeTime);
    }
}