using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CannonBoss : Shooter, IDamage
{
    [Header("--CANNON STATS--")]
    [SerializeField] GameObject _bomberPref;
    [SerializeField] int ShootForce;
    [SerializeField] GameObject DeathTrigger;

    private List<GameObject> Bombers = new List<GameObject>();

    [SerializeField] int shootDelay;
    private int ShootSteps;
    private GameObject bomberB;

    protected override void Start()
    {
        base.Start();
        steps = stepsOriginal;
        ShootSteps = stepsOriginal + shootDelay;
    }
    protected override void Restart()
    {
        base.Restart();
        steps = stepsOriginal;
        ShootSteps = stepsOriginal + shootDelay;
        foreach (var bomber in Bombers)
        {
            IDamage damageable = bomber.GetComponent<IDamage>();

            if (damageable != null)
            {
                damageable.takeDamage(1);
            }
        }
        DeathTrigger.SetActive(true);
    }

    protected override void BoopImpulse(Vector3 origin, float force, bool slam = false)
    {
        Damage(1);
    }
    protected override IEnumerator Death()
    {
        yield return base.Death();
        DeathTrigger.SetActive(false);
        foreach(var bomber in Bombers)
        {
            IDamage damageable = bomber.GetComponent<IDamage>();

            if (damageable != null)
            {
                damageable.takeDamage(1);
            }
        }
    }
    protected override void BeatAction()
    {
        if (enemyCol.enabled)
        {
            steps--;
            ShootSteps--;
            if (steps == 0)
            {
                _bomberPref.transform.localScale = new Vector3(0.09199633f, 0.09199633f, 0.09199633f);

                // Check if there's an inactive bomber in the list
                //GameObject inactiveBomber = Bombers.Find(bomber => !bomber.GetComponent<Collider>().enabled);
                //if (inactiveBomber != null)
                //{
                //    // Reuse the inactive bomber
                //    bomberB = inactiveBomber;
                //    bomberB.transform.position = shootPos.position;
                //    bomberB.transform.rotation = shootPos.rotation;
                //    bomberB.transform.parent = transform; // Set the bomber as a child of the Cannon Boss
                //}
                //else
                //{
                //    bomberB = Instantiate(_bomberPref, shootPos.position, shootPos.rotation, transform);
                //    Bombers.Add(bomberB);
                //}
                bomberB = Instantiate(_bomberPref, shootPos.position, shootPos.rotation, transform);
                Bombers.Add(bomberB);
            }
            if (ShootSteps <= 0)
            {
                bomberB.transform.parent = null;
                Rigidbody bomberRB = bomberB.GetComponent<Rigidbody>();
                if (bomberRB != null)
                {
                    bomberRB.AddForce(shootPos.transform.forward * ShootForce, ForceMode.Impulse);
                }
                steps = stepsOriginal;
                ShootSteps = stepsOriginal + shootDelay;
            }
        }
    }
}
