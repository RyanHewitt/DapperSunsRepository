using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Shooter : EnemyAi
{
    [Header("---Shooter---")]
    [SerializeField] protected int stepsOriginal;
    [SerializeField] protected Transform shootPos;
    protected int steps;
    protected override void Start()
    {
        base.Start();
        steps = stepsOriginal;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Restart()
    {
        base.Restart();
        steps = stepsOriginal;
    }

    protected override void Move()
    {

    }

    protected override void Damage(int amount)
    {
        base.Damage(amount);
    }

    protected override IEnumerator Death()
    {
        yield return base.Death();
    }

    protected override void BeatAction()
    {
        if (playerInRange && !GameManager.instance.playerDead && enemyCol.enabled)
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

    protected override void Rotate()
    {
        Vector3 targetDirection = playerDirection;
        if (targetDirection != Vector3.zero)
        {
            // Horizontal rotation (side to side)
            Quaternion horizontalRotation = Quaternion.LookRotation(targetDirection);

            // Vertical rotation (up and down)
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.magnitude) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -maxVerticalAngle, maxVerticalAngle);

            Quaternion verticalRotation = Quaternion.Euler(-angle, 0, 0);

            // Combine both rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, horizontalRotation * verticalRotation, Time.deltaTime * PlayerFaceSpeed);
        }
    }

    protected override void BoopImpulse(Vector3 origin, float force, bool slam = false)
    {
        rb.AddForce(-(origin - transform.position).normalized * force * boopMultiplier, ForceMode.Impulse);
    }
}