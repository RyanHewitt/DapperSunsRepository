using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : EnemyAi
{
    [Header("---Bomb Stats---")]
    [SerializeField] Collision collision;
    [SerializeField] float explosionRadius; // Radius of explosion
    [SerializeField] int explosionForce;    // Force inflicted by the explosion
    [SerializeField] int countdown;
    [SerializeField] int cooldown;

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
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();
        
        Explode();
        yield return new WaitForSeconds(cooldown);
        startCountdown = true;
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

    protected override void BoopImpulse(float force, bool slam = false)
    {
        rb.AddForce(-playerDirection * force * boopMultiplier, ForceMode.Impulse);
        startCountdown = true;
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            IBoop boopable = colliders[i].GetComponent<IBoop>();
            if (boopable != null)
            {
                boopable.DoBoop(explosionForce);
            }
        }

        startCountdown = false;
        counter = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;
        StartCoroutine(Death());
    }
}
