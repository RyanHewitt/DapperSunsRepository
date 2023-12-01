using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBomber : Bomber
{
    protected override void BeatAction()
    {
        if(gameObject.activeInHierarchy)
        {
            base.BeatAction();
        }
    }
}
