using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            RailGunBoss railgunboss = FindObjectOfType<RailGunBoss>();
            if(railgunboss != null)
            {
                railgunboss.SetTarget(transform);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RailGunBoss railGunBoss = FindObjectOfType<RailGunBoss>();

            if (railGunBoss != null)
            {
                railGunBoss.ClearTarget(transform);
            }
        }
    }
}
