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
    [SerializeField] int expcountdown;

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
        yield return new WaitForSeconds(expcountdown);

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
        StartCoroutine(Death());
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
        baseModel.enabled = false;
        outlineModel.enabled = false;
        enemyCol.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
