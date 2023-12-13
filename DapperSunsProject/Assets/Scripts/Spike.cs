using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] int SpikeDMG;
    [SerializeField] Color up;
    [SerializeField] Color down;
    [SerializeField] Color warning;
    [SerializeField] GameObject spikeObj;
    [SerializeField] int warnBeat;
    [SerializeField] int onBeat;
    [SerializeField] int offBeat;

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
        mat.color = down;
        mat.SetColor("_EmissionColor", down);

        spikeObj.SetActive(false);
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
            damageable.takeDamage(SpikeDMG);
        }
    }

    void DoBeat()
    {
        beatCounter++;
        if (beatCounter == warnBeat) // Warn on second beat
        {
            mat.color = warning;
            mat.SetColor("_EmissionColor", warning);
        }
        else if (beatCounter == offBeat)
        {
            beatCounter = 0;
            coll.enabled = false;
            isUp = false;
            mat.color = down;
            mat.SetColor("_EmissionColor", down);

            spikeObj.SetActive(false);
        }
        else if (beatCounter == onBeat)
        {
            coll.enabled = true;
            isUp = true;
            mat.color = up;
            mat.SetColor("_EmissionColor", up);

            spikeObj.SetActive(true);
        }
    }
}