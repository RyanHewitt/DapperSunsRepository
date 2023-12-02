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
            //// Check if the bomber is inactive
            //if (!bomber.activeSelf)
            //{
            //    // Reset the position ,rotation ,and parent
            //    bomber.transform.position = shootPos.position;
            //    bomber.transform.rotation = shootPos.rotation;
            //    bomber.transform.parent = transform;
            //    bomber.SetActive(false);
            //    // Continue with the next bomber
            //    continue;
            //}

            // If the bomber is active, set it as inactive
            bomber.SetActive(false);
        }
    }

    protected override void BoopImpulse(Vector3 origin, float force, bool slam = false)
    {
        Damage(1);
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
                GameObject inactiveBomber = Bombers.Find(bomber => !bomber.activeSelf);
                if (inactiveBomber != null)
                {
                    // Reuse the inactive bomber
                    bomberB = inactiveBomber;
                    bomberB.transform.position = shootPos.position;
                    bomberB.transform.rotation = shootPos.rotation;
                    bomberB.transform.parent = transform; // Set the bomber as a child of the Cannon Boss
                    bomberB.SetActive(true);
                }
                else
                {
                    bomberB = Instantiate(_bomberPref, shootPos.position, shootPos.rotation, transform);
                    Bombers.Add(bomberB);
                }
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
}
