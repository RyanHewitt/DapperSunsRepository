using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : EnemyAi
{
    [Header("---Heavy---")]
    [SerializeField] public Transform[] shootPositions;
    [SerializeField] public int steps;

    public int currentStep;
    public Collider groundTrigger;

    protected override void Start()
    {
        base.Start();

        groundTrigger = outline.GetComponent<Collider>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Restart()
    {
        base.Restart();

        if (groundTrigger != null)
        {
            groundTrigger.enabled = true; 
        }
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();

        groundTrigger.enabled = false;
    }

    protected override void BeatAction()
    {
        if (playerInRange && !GameManager.instance.playerDead && enemyCol.enabled)
        {
            currentStep++;
            if (steps <= currentStep)
            {
                AudioManager.instance.Play3D(ShootAudio, transform.position);

                //Shoot bullets on all four sides of enemy
                foreach (Transform pos in shootPositions)
                {
                    Instantiate(bullet, pos.position, pos.rotation);
                }

                currentStep = 0;
            }
        }
    }

    protected override void BoopImpulse(Vector3 origin, float force, bool slam = false)
    {
        if (slam)
        {
            Damage(1);
        }
    }
}