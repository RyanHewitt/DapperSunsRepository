using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : EnemyAi
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

    protected override void Move()
    {
        // IF PLAYER IN RANGE MOVE TOWARD THEM
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

    }
}