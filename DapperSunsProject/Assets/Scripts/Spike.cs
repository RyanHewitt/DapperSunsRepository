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

    Vector3 upPos;
    Vector3 downPos;

    bool isUp = false;

    int beatCounter;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        beatCounter = 0;

        upPos = spikeObj.transform.position;
        downPos = new Vector3(upPos.x, upPos.y - 3, upPos.z);

        coll = GetComponent<Collider>();
        coll.enabled = false;
        spikeObj.transform.position = downPos;

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
                spikeObj.transform.position = downPos;
                coll.enabled = false;
                isUp = false;
                mat.color = down;
                mat.SetColor("_EmissionColor", down);

                spikeObj.SetActive(false);
            }
            else
            {
                spikeObj.transform.position = upPos;
                coll.enabled = true;
                isUp = true;
                mat.color = up;
                mat.SetColor("_EmissionColor", up);

                spikeObj.SetActive(true);
            }
        }
    }
}