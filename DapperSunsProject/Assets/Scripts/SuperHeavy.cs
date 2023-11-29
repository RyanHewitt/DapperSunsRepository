using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperHeavy : Heavy
{
    [Header("---SuperHeavy---")]
    [SerializeField] private int superHeavyHitPoints = 3; // Takes three hits to kill
    //[SerializeField] private float knockbackForce = 10f; // Force to knock the player back
    [SerializeField] private float trackingBulletSpeed = 2f; // Speed for tracking bullets
    [SerializeField] private float trackingRate = 1f; // Tracking rate for bullets

    protected override void Start()
    {
        base.Start();
        HP = superHeavyHitPoints; 
    }

    protected override void BeatAction()
    {
        if (playerInRange && !GameManager.instance.playerDead && enemyCol.enabled)
        {
            currentStep++;
            if (steps <= currentStep)
            {
                ShootTrackingBullets(); // SuperHeavy uses tracking bullets
                currentStep = 0;
            }
        }
    }

    private void ShootTrackingBullets()
    {
        foreach (Transform shootPosition in shootPositions)
        {
            GameObject bulletObject = Instantiate(bullet, shootPosition.position, shootPosition.rotation);
            Rigidbody bulletRb = bulletObject.GetComponent<Rigidbody>();
            bulletRb.velocity = shootPosition.forward * trackingBulletSpeed;

            // Add tracking component or modify bullet behavior for tracking
            TrackingBullet tracker = bulletObject.AddComponent<TrackingBullet>();
            tracker.target = GameManager.instance.player.transform;
            tracker.trackingRate = trackingRate;
        }
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);
        // Implement knockback 
    }

    
}
