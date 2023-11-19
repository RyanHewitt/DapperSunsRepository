using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : EnemyAi
{
    [Header("---Bomber Stats---")]
    [SerializeField] float explosionRadius; // Radius of explosion
    [SerializeField] float moveSpeed;       // Speed at which the bomber moves towards the player
    [SerializeField] int explosionForce;    // Force inflicted by the explosion
    [SerializeField] int countdown;

    bool startCountdown;
    int counter;

    [Header("---Explosion Effects---")]
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioClip countSound;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (playerInRange && enemyCol.enabled)
        {
            startCountdown = true;
        }
    }

    protected override void Restart()
    {
        base.Restart();

        startCountdown = false;
        counter = 0;
    }

    protected override void Move()
    {
        Vector3 directionToPlayer = (GameManager.instance.player.transform.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
    }

    protected override void Rotate()
    {
        Vector3 targetDirection = playerDirection;
        if (targetDirection != Vector3.zero)
        {
            // Horizontal rotation (side to side)
            Quaternion horizontalRotation = Quaternion.LookRotation(targetDirection);

            // Vertical rotation (up and down)
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.magnitude) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -maxVerticalAngle, maxVerticalAngle);

            Quaternion verticalRotation = Quaternion.Euler(-angle, 0, 0);

            // Combine both rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, horizontalRotation * verticalRotation, Time.deltaTime * PlayerFaceSpeed);
        }
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();

        Explode();
    }

    protected override void BeatAction()
    {

        if (startCountdown)
        {
            counter++;
            AudioManager.instance.Play3D(countSound, transform.position);
            StartCoroutine(Flash());
            if (counter == 0)
            {
                
                AudioManager.instance.Play3D(countSound, transform.position);
            }

            if (counter >= countdown)
            {
                StartCoroutine(Death());
            }
        }
    }

    protected override void BoopImpulse(float force, bool slam = false)
    {
        rb.AddForce(-playerDirection * force * boopMultiplier, ForceMode.Impulse);
    }

    void Explode()
    {
    
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // Check if the player is close enough to be knocked back
        if (distanceToPlayer <= explosionRadius)
        {
            GameManager.instance.playerScript.DoBoop(playerDirection * explosionForce);
        }

        startCountdown = false;
        counter = 0;
    }
}