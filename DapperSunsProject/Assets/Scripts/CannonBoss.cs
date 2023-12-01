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
            Destroy(bomber);
        }
    }

    protected override void BoopImpulse(float force, bool slam = false)
    {
        base.BoopImpulse(force, slam);
        Damage(HP);
    }
    protected override void BeatAction()
    {
        steps--;
        ShootSteps--;
        if (steps == 0)
        {
            _bomberPref.transform.localScale = new Vector3(0.09199633f, 0.09199633f, 0.09199633f);
            bomberB = Instantiate(_bomberPref, shootPos.position, shootPos.rotation, transform);
            Bombers.Add(bomberB);
        }
        if (ShootSteps <= 0)
        {
            bomberB.transform.parent = null;
            Rigidbody bomberRB = bomberB.GetComponent<Rigidbody>();
            if (bomberRB != null)
            {
                bomberRB.AddForce(transform.forward * ShootForce, ForceMode.Impulse);
            }
            steps = stepsOriginal;
            ShootSteps = stepsOriginal + shootDelay;
        }
    }

}
