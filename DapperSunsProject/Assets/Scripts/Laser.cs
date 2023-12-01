using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] int LazerDamage;
    [SerializeField] GameObject warningLazer;
    [SerializeField] GameObject LAZER;

    [SerializeField] int WarnSteps;
    [SerializeField] int OutSteps;
    [SerializeField] bool IsAlwaysOn;

    Collider coll;

    bool isUp = false;

    int beatCounter;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        beatCounter = 0;

        coll = GetComponent<Collider>();
        coll.enabled = false;

        warningLazer.SetActive(false);
        LAZER.SetActive(false);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.takeDamage(LazerDamage);
        }
    }

    void DoBeat()
    {
        if (IsAlwaysOn)
        {
            LAZER.SetActive(true);
        }
        if (!IsAlwaysOn)
        {
            beatCounter++;
            if (beatCounter == WarnSteps) // Warn on beat
            {
                warningLazer.SetActive(true);
            }
            else if (beatCounter % OutSteps == 0) // Go up or down every beat
            {
                if (isUp)
                {
                    beatCounter = 0;
                    coll.enabled = false;
                    isUp = false;
                    LAZER.SetActive(false);

                }
                else
                {
                    coll.enabled = true;
                    isUp = true;
                    warningLazer.SetActive(false);
                    LAZER.SetActive(true);
                }
            } 
        }
    }
}
