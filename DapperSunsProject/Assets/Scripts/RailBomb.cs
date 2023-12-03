using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RailBomb : Bomb
{
    bool disabled;

    protected override void Restart()
    {
        base.Restart();

        disabled = false;
    }

    protected override void Explode()
    {
        base.Explode();

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Boss"))
            {
                disabled = true;
                break;
            }
        }
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();
        
        if (disabled)
        {
            startCountdown = false;
        }
    }
}
