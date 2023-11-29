using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RailGunBoss : Shooter
{
    [SerializeField] LineRenderer laserLineRenderer;
    [SerializeField] Transform LazerPosition;
    [SerializeField] GameObject lazer;

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

    protected override void Rotate()
    {
        if(playerDirection !=null)
        {
            Vector3 targetDirection = playerDirection;

            targetDirection.y = 0f; // Ignore the vertical component

            if (targetDirection != Vector3.zero)
            {
                Quaternion horizontalRotation = Quaternion.LookRotation(targetDirection);

                float yRotation = horizontalRotation.eulerAngles.y;

                Quaternion sideToSideRotation = Quaternion.Euler(0f, yRotation, 0f);

                // Combine both rotations
                transform.rotation = Quaternion.Slerp(transform.rotation, sideToSideRotation, Time.deltaTime * PlayerFaceSpeed);
            }
        }

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
    }
}
