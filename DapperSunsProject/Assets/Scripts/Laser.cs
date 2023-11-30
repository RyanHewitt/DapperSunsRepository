using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] int LazerDamage;
    [SerializeField] Color _out;
    [SerializeField] Color _in;
    [SerializeField] GameObject warningLazer;
    [SerializeField] GameObject LAZER;

    [SerializeField] int WarnSteps;
    [SerializeField] int OutSteps;

    Collider coll;

    Material mat;


    bool isUp = false;

    int beatCounter;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        beatCounter = 0;

        coll = GetComponent<Collider>();
        coll.enabled = false;

        mat = GetComponent<Renderer>().material;
        mat.color = _in;
        mat.SetColor("_EmissionColor", _in);

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
                mat.color = _in;
                mat.SetColor("_EmissionColor", _in);
                LAZER.SetActive(false);

            }
            else
            {
                coll.enabled = true;
                isUp = true;
                mat.color = _out;
                mat.SetColor("_EmissionColor", _out);
                warningLazer.SetActive(false);
                LAZER.SetActive(true);
            }
        }
    }
}
