using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathtrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.takeDamage(500);
        }
    }
}
