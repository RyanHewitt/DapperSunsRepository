using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : EnemyAi
{
    [Header("---Bomb Stats---")]
    [SerializeField] Collision collision;
    [SerializeField] Renderer fuseModel;
    [SerializeField] float explosionRadius; // Radius of explosion
    [SerializeField] int explosionForce;    // Force inflicted by the explosion
    [SerializeField] int countdown;
    [SerializeField] int cooldown;

    protected Collider[] colliders;
    protected bool startCountdown;
    int counter;

    [Header("---Explosion Effects---")]
    [SerializeField] GameObject explosionEffect;
    [SerializeField] AudioClip countSound;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        playerDirection = GameManager.instance.player.transform.position - transform.position;

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
        fuseModel.enabled = true;
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();
        
        Explode();
        startCountdown = true;
        fuseModel.enabled = false;
    }

    protected override void BeatAction()
    {
        if (startCountdown)
        {
            counter++;
            if (enemyCol.enabled)
            {
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
            else
            {
                if (counter >= countdown)
                {
                    startCountdown = false;
                    Restart();
                }
            }
        }
    }

    protected override void BoopImpulse(Vector3 origin, float force, bool slam = false)
    {
        rb.AddForce(-(origin - transform.position).normalized * force * boopMultiplier, ForceMode.Impulse);
        startCountdown = true;
    }

    protected virtual void Explode()
    {
        colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            IBoop boopable = colliders[i].GetComponent<IBoop>();
            if (boopable != null)
            {
                boopable.DoBoop(transform.position, explosionForce);
            }
        }

        startCountdown = false;
        counter = 0;

        Instantiate(explosionEffect, transform.position, transform.rotation);
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;
        StartCoroutine(Death());
    }
}