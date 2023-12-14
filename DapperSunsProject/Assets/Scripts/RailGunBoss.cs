using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RailGunBoss : Shooter
{
    [SerializeField] LineRenderer laserLineRenderer;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] Transform LazerPosition;
    [SerializeField] GameObject lazer;
    private Transform currentTarget;
    bool _playerInRange;

    protected override void Start()
    {
        base.Start();
        lazer.SetActive(true);
        laserLineRenderer.enabled = true;
    }

    protected override void Update()
    {
        playerDirection = GameManager.instance.player.transform.position - transform.position;
        Rotate();
        Move();

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

    protected override void BoopImpulse(Vector3 origin, float force, bool slam = false)
    {
        takeDamage(1);
        AudioManager.instance.Play3D(hurtSound, transform.position, 1, 0.5f, 1);
        StartCoroutine(Flash());
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

    public void SetPlayerInRange(bool inRange)
    {
        _playerInRange = inRange;
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
        if (_playerInRange && !GameManager.instance.playerDead && enemyCol.enabled)
        {
            if (CheckLineOfSight(GameManager.instance.player.transform)) // Check line of sight
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