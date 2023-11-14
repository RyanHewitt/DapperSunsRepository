using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : EnemyAi
{
    [Header("---Bomber Stats---")]
    [SerializeField] float explosionRadius = 5.0f; // Radius to detect the player
    [SerializeField] float moveSpeed = 3.0f;       // Speed at which the bomber moves towards the player
    [SerializeField] int explosionDamage = 50;     // Damage inflicted by the explosion

    bool startCountdown;
    int counter;

    [Header("---Explosion Effects---")]
    public AudioClip explosionAudioClip;

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
        if(startCountdown)
        {
            counter++;
            if(counter >= 4)
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
        AudioManager.instance.Play3D(explosionAudioClip, transform.position);
    
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // Check if the player is close enough to be damaged and knocked back
        if (distanceToPlayer <= explosionRadius) // Adjust this value based on the desired explosion radius
        {
            // Damage the player
            GameManager.instance.player.GetComponent<IDamage>().takeDamage(explosionDamage);

            // Knockback the player
            
        }


        startCountdown = false;
        counter = 0;

        //add effects
    }

    void StartExplosionCountdown()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
        if (distanceToPlayer <= 2.0f) // Check if still close to the player
        {
            Explode();
        }
    }

}