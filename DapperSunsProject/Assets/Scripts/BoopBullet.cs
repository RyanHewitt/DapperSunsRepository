using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoopBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float bulletSpeed;
    [SerializeField] float life = 3;

    public void Awake()
    {
        rb.velocity = -transform.forward * bulletSpeed;
        transform.localRotation *= Quaternion.Euler(90, 0, 0);
        Destroy(gameObject, life);
    }
}