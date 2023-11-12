using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : EnemyAi
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Restart()
    {
        base.Restart();
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
    }

    protected override void BeatAction()
    {
        if (playerInRange && !GameManager.instance.playerDead && enemyCol.enabled)
        {
            AudioManager.instance.Play3D(ShootAudio, transform.position);

            //Shoot bullets on all four sides of enemy
            //Instantiate(bullet, shootPos.position, transform.rotation);
        }
    }

    protected override void BoopImpulse(float force)
    {
        // IF PLAYER IS IN TOP TRIGGER, DAMAGE ENEMY
    }
}