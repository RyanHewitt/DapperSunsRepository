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
        if (beatCounter == 2) // Warn on second beat
        {
            mat.color = warning;
            mat.SetColor("_EmissionColor", warning);
        }
        else if (beatCounter % 4 == 0) // Go up or down every fourth beat
        {
            if (isUp)
            {
                beatCounter = 0;
                coll.enabled = false;
                isUp = false;
                mat.color = down;
                mat.SetColor("_EmissionColor", down);

                spikeObj.SetActive(false);
            }
            else
            {
                coll.enabled = true;
                isUp = true;
                mat.color = up;
                mat.SetColor("_EmissionColor", up);

                spikeObj.SetActive(true);
            }
        }
    }
}