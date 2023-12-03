using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTarget : MonoBehaviour
{
    private bool playerInRange;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            RailGunBoss railgunboss = FindObjectOfType<RailGunBoss>();
            if(railgunboss != null)
            {
                playerInRange = true;
                NotifyRailGunBoss();
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
                playerInRange = false;
                railGunBoss.ClearTarget(transform);
                NotifyRailGunBoss();
            }
        }
    }
    private void NotifyRailGunBoss()
    {
        RailGunBoss railGunBoss = FindObjectOfType<RailGunBoss>();

        if (railGunBoss != null)
        {
            railGunBoss.SetPlayerInRange(playerInRange);
        }
    }
}
