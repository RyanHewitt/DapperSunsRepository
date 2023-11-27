using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperHeavy : EnemyAi
{
    [Header("---SuperHeavy---")]
    [SerializeField] Transform[] shootPositions;
    [SerializeField] GameObject bulletPrefab; // Prefab for the bullets
    //[SerializeField] float bulletSpeed = 2f; // Lower speed for more trackable bullets
    //[SerializeField] float knockbackForce = 10f; // Higher force for more impactful knockback
    [SerializeField] int maxHitPoints = 3; // It takes three hits to kill the SuperHeavy

    public int currentHitPoints;
    public bool isDead = false;

    protected override void Start()
    {
        base.Start();
        currentHitPoints = maxHitPoints;
    }

    protected override void Update()
    {
        base.Update();
        
    }

    protected override void Damage(int amount)
    {
        if (isDead) return;

        currentHitPoints -= amount;
        if (currentHitPoints <= 0)
        {
            StartCoroutine(Death());
        }
        else
        {
            // Possibly trigger a knockback animation or effect
           
        }
    }

    protected override IEnumerator Death()
    {
        if (isDead) yield break;

        isDead = true;
        // Perform any special death effects, animations, or sounds

        yield return new WaitForSeconds(1f); // Placeholder for death effects duration
        // After death effects, do cleanup if necessary

        base.Death();
    }

    protected override void BeatAction()
    {
        // SuperHeavy-specific beat action logic
       
        ShootTrackingBullets();
    }

    protected override void BoopImpulse(float force, bool slam = false)
    {
        if (slam)
        {
            Damage(1);
            // Apply knockback to the player
        }
    }

    public void ShootTrackingBullets()
    {
       
    }

    public void KnockPlayerBack(Vector3 direction)
    {
        
    }
}
