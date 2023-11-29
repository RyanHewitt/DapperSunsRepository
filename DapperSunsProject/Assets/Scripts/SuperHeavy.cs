using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuperHeavy : Heavy
{
    [Header("---SuperHeavy---")]
    [SerializeField] private int superHeavyHitPoints = 3; // Takes three hits to kill
    [SerializeField] private float teleportDistance = 5.0f; // Distance to teleport away from the player
    //[SerializeField] private float knockbackForce = 10f; // Force to knock the player back
    [SerializeField] private float trackingBulletSpeed = 2f; // Speed for tracking bullets
    [SerializeField] private float trackingRate = 1f; // Tracking rate for bullets

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

        if (HP > 0)
        {
            TeleportAwayFromPlayer();
        }
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
        Vector3 directionAwayFromPlayer = (transform.position - GameManager.instance.player.transform.position).normalized;
        Vector3 potentialTeleportPosition = transform.position + directionAwayFromPlayer * teleportDistance;

        if (IsPositionValid(potentialTeleportPosition))
        {
            transform.position = potentialTeleportPosition; // Teleport the SuperHeavy
        }
        else
        {
            // Fallback: Try to find an alternative valid position
            Vector3 alternativePosition = FindAlternativePosition();
            if (alternativePosition != Vector3.zero)
            {
                transform.position = alternativePosition;
            }
           
        }
    }

    private bool IsPositionValid(Vector3 position)
    {
        float checkRadius = 1.0f; 

   
        Collider[] hitColliders = Physics.OverlapSphere(position, checkRadius);

        return hitColliders.Length == 0; 
    }

    private Vector3 FindAlternativePosition()
    {
        
        Vector3[] directions = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        foreach (var dir in directions)
        {
            Vector3 potentialPosition = transform.position + dir * teleportDistance;
            if (IsPositionValid(potentialPosition))
            {
                return potentialPosition;
            }
        }
        return Vector3.zero; 
    }
}
