using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : EnemyAi
{
    [Header("---Bomber Stats---")]
    public float detectionRadius = 5.0f; // Radius to detect the player
    public float moveSpeed = 3.0f;       // Speed at which the bomber moves towards the player
    public int explosionDamage = 50;     // Damage inflicted by the explosion

    [Header("---Explosion Effects---")]
    public AudioClip explosionAudioClip;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (playerInRange)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
            if (distanceToPlayer <= detectionRadius)
            {
                Move();

                // Start the explosion countdown when the player is close enough
                if (!IsInvoking(nameof(StartExplosionCountdown)))
                {
                    Invoke(nameof(StartExplosionCountdown), 3.0f); // Wait for 3 seconds
                }
            }
        }
    }

    protected override void Restart()
    {
        base.Restart();
    }

    protected override void Move()
    {
        Vector3 directionToPlayer = (GameManager.instance.player.transform.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();

        outlineMat.color = flashColor;
        outlineMat.SetColor("_EmissionColor", flashColor);

        yield return new WaitForSeconds(0.1f);

        outlineMat.color = baseColor;
        outlineMat.SetColor("_EmissionColor", baseColor);

        baseModel.enabled = false;
        outlineModel.enabled = false;
        enemyCol.enabled = false;

        Explode();
    }

    protected override void BeatAction()
    {
        
    }

    protected override void BoopImpulse(float force, bool slam = false)
    {
        rb.AddForce(-playerDirection * force * boopMultiplier, ForceMode.Impulse);
    }

    void Explode()
    {
        if (explosionAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position);
        }
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // Check if the player is close enough to be damaged and knocked back
        if (distanceToPlayer <= 1.0f) // Adjust this value based on the desired explosion radius
        {
            // Damage the player
            GameManager.instance.player.GetComponent<IDamage>().takeDamage(explosionDamage);

            // Knockback the player
            Rigidbody playerRb = GameManager.instance.player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDirection = (GameManager.instance.player.transform.position - transform.position).normalized;
                float knockbackForce = 500; // Adjust this value to set the knockback strength
                playerRb.AddForce(knockbackDirection * knockbackForce);
            }
        }


        //add Audio

        //add effects


        StartCoroutine(Death());
    }

    void StartExplosionCountdown()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
        if (distanceToPlayer <= 1.0f) // Check if still close to the player
        {
            Explode();
        }
    }

}