using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBullet : Bullet
{
    [SerializeField] private float trackingRate = 1.0f;

    Transform target;

    protected override void Start()
    {
        base.Start();

        target = GameManager.instance.player.transform;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToTarget, trackingRate * Time.deltaTime);

            rb.velocity = transform.forward * bulletSpeed;
        }
    }
}