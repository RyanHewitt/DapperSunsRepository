using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuperHeavy : Heavy
{
    [SerializeField] AudioClip hurtSound;
    [Header("---Teleport Positions---")]
    [SerializeField] Transform[] teleportPos;

    int teleportIndex = 0;

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
                AudioManager.instance.Play3D(ShootAudio, transform.position, 1, 0.5f, 3);
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

        teleportIndex = 0;
    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);

        AudioManager.instance.Play3D(hurtSound, transform.position, 1, 0.5f, 1);
        TeleportAwayFromPlayer();
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();
    }

    void TeleportAwayFromPlayer()
    {
        transform.position = teleportPos[teleportIndex].position;
        if (teleportIndex < teleportPos.Length - 1)
        {
            teleportIndex++; 
        }
        else
        {
            teleportIndex = 0;
        }
    }
}