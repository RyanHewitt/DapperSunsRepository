using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuperHeavy : Heavy
{
    [Header("---SuperHeavy---")]
    [SerializeField] private int superHeavyHitPoints = 3; // Takes three hits to kill
    //[SerializeField] private float knockbackForce = 10f; // Force to knock the player back
    [SerializeField] private float trackingBulletSpeed = 2f; // Speed for tracking bullets
    [SerializeField] private float trackingRate = 1f; // Tracking rate for bullets

    [Header("---Teleport Positions---")]
    [SerializeField] private Transform teleportPosition1; 
    [SerializeField] private Transform teleportPosition2; 

    private bool lastTeleportWasPos1 = false;

    protected override void Start()
    {
        base.Start();
        HP = superHeavyHitPoints;
        
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void BeatAction()
    {
        base.BeatAction();
        if (playerInRange && !GameManager.instance.playerDead && enemyCol.enabled)
        {
            currentStep++;
            if (steps <= currentStep)
            {
                ShootTrackingBullets();
                currentStep = 0;
            }
        }
    }

    protected override void Restart()
    {
        base.Restart();

    }

    private void ShootTrackingBullets()
    {
        foreach (Transform shootPosition in shootPositions)
        {
            GameObject bulletObject = Instantiate(bullet, shootPosition.position, shootPosition.rotation);
            Rigidbody bulletRb = bulletObject.GetComponent<Rigidbody>();
            bulletRb.velocity = shootPosition.forward * trackingBulletSpeed;


            TrackingBullet tracker = bulletObject.AddComponent<TrackingBullet>();
            tracker.target = GameManager.instance.player.transform;
            tracker.trackingRate = trackingRate;
        }
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);

        TeleportAwayFromPlayer();
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();

        
    }

    protected override void BoopImpulse(float force, bool slam = false)
    {
       base.BoopImpulse(force, slam);
        
    }

    private void TeleportAwayFromPlayer()
    {
        
        Transform targetTeleportPosition = lastTeleportWasPos1 ? teleportPosition2 : teleportPosition1;

       
        transform.position = targetTeleportPosition.position;

        
        lastTeleportWasPos1 = !lastTeleportWasPos1;
    }
}


