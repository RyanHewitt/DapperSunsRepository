using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBomber : Bomber
{
    private void OnCollisionEnter(Collision collision)
    {
        Damage(1);
    }
}
