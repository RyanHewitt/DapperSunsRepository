using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Shooter
{
    public LineRenderer laserLineRenderer;
    public Transform LazerPosition;
    public GameObject lazer;

    protected override void Start()
    {
        base.Start();
        lazer.SetActive(true);
        laserLineRenderer.enabled = true;
    }

    protected override void Update()
    {
        base.Update();

        Vector3 aimDirection = LazerPosition.forward;

        Ray ray = new Ray(transform.position, aimDirection);
        RaycastHit hit;

        // Check if the ray hits an object.
        if (Physics.Raycast(ray, out hit))
        {
            // Enable the laser and set its positions.
            laserLineRenderer.SetPosition(0, transform.position);
            laserLineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            laserLineRenderer.SetPosition(0, transform.position);
            laserLineRenderer.SetPosition(1, transform.position + transform.forward * 9999);
        }
    }

    protected override void Restart()
    {
        base.Restart();
        laserLineRenderer.enabled = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            steps = stepsOriginal;
        }
        base.OnTriggerEnter(other);
    }

    protected override IEnumerator Death()
    {
        laserLineRenderer.enabled = false;
        return base.Death();
    }

    protected override void BeatAction()
    {
        base.BeatAction();
        if (playerInRange && !GameManager.instance.playerDead && enemyCol.enabled)
        {
            if (CheckLineOfSight(GameManager.instance.player.transform))
            {
                steps--;
                if (steps <= 0)
                {
                    steps = stepsOriginal;

                    AudioManager.instance.Play3D(ShootAudio, transform.position);
                    Instantiate(bullet, shootPos.position, transform.rotation);
                }
            }
        }
    }
}