using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperHeavy : EnemyAi
{
    [Header("---SuperHeavy---")]
    [SerializeField] Transform[] shootPositions;
    [SerializeField] int steps;

  

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

        
    }

    protected override void BeatAction()
    {
     
    }

    protected override void BoopImpulse(float force, bool slam = false)
    {
        if (slam)
        {
            Damage(1);
        }
    }
}