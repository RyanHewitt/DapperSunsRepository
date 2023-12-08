using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuperHeavy : Heavy
{
    [Header("---Teleport Positions---")]
    [SerializeField] private Transform teleportPosition1; 
    [SerializeField] private Transform teleportPosition2; 

    private bool lastTeleportWasPos1 = false;

    protected override void Start()
    {
        base.Start();        
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void BeatAction()
    {
        if (!GameManager.instance.playerDead && enemyCol.enabled)
        {
            currentStep++;
            if (steps <= currentStep)
            {
                foreach (Transform pos in shootPositions)
                {
                    Instantiate(bullet, pos.position, pos.rotation);
                }
                currentStep = 0;
            }
        }
    }

    protected override void Restart()
    {
        base.Restart();
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);

        TeleportAwayFromPlayer();
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();

        gameObject.SetActive(false);
    }

    void TeleportAwayFromPlayer()
    {
        Transform targetTeleportPosition = lastTeleportWasPos1 ? teleportPosition2 : teleportPosition1;
       
        transform.position = targetTeleportPosition.position;

        lastTeleportWasPos1 = !lastTeleportWasPos1;
    }
}