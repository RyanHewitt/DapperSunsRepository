using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] Rigidbody rb;
    [SerializeField] float bulletSpeed;
    [SerializeField] float life = 3;

    public void Awake()
    {
        rb.velocity = transform.forward * bulletSpeed;
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.takeDamage(damage);
        }

        Destroy(gameObject);
    }
}