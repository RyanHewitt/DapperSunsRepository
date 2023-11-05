using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : Beat
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

    protected override void Start()
    {
        base.Start();

        beatCounter = 0;

        upPos = spikeObj.transform.position;
        downPos = new Vector3(upPos.x, upPos.y - 3, upPos.z);

        coll = GetComponent<Collider>();
        coll.enabled = false;
        spikeObj.transform.position = downPos;

        mat = GetComponent<Renderer>().material;
        mat.color = down;
        mat.SetColor("_EmissionColor", down);
    }

    protected override void Update()
    {
        base.Update();
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

    protected override void DoBeat()
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
            }
            else
            {
                spikeObj.transform.position = upPos;
                coll.enabled = true;
                isUp = true;
                mat.color = up;
                mat.SetColor("_EmissionColor", up);
            }
        }
    }
}