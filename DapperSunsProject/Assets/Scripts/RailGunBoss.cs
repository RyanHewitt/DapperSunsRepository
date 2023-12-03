using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RailGunBoss : Shooter
{
    [SerializeField] LineRenderer laserLineRenderer;
    [SerializeField] Transform LazerPosition;
    [SerializeField] GameObject lazer;
    private Transform currentTarget;

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

        Ray ray = new Ray(LazerPosition.position, aimDirection);
        RaycastHit hit;

        // Check if the ray hits an object.
        if (Physics.Raycast(ray, out hit))
        {
            // Enable the laser and set its positions.
            laserLineRenderer.SetPosition(0, LazerPosition.position);
            laserLineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            laserLineRenderer.SetPosition(0, LazerPosition.position);
            laserLineRenderer.SetPosition(1, LazerPosition.position + LazerPosition.forward * 9999);
        }
    }

    protected override void Restart()
    {
        base.Restart();
        laserLineRenderer.enabled = true;
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    public void ClearTarget(Transform target)
    {
        if (currentTarget == target)
        {
            currentTarget = null;
        }
    }

    protected override void Rotate()
    {
        if (currentTarget != null)
        {
            Vector3 targetDirection = currentTarget.position - transform.position;
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
