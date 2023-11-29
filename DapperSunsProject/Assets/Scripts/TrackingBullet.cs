using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBullet : MonoBehaviour
{
    public Transform target;
    public float trackingRate = 1.0f;
    private Rigidbody rb;
    private float bulletSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        bulletSpeed = rb.velocity.magnitude; // Capture the initial speed of the bullet
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            rb.velocity = Vector3.RotateTowards(rb.velocity, directionToTarget * bulletSpeed, trackingRate * Time.deltaTime, 0.0f);
        }
    }
}

