using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : Beat
{
    [SerializeField] int SpikeDMG;
    [SerializeField] Color up;
    [SerializeField] Color down;
    [SerializeField] GameObject spikeObj;
    
    Collider coll;

    Vector3 upPos;
    Vector3 downPos;

    bool isUp = false;

    protected override void Start()
    {
        base.Start();

        upPos = spikeObj.transform.position;
        downPos = new Vector3(upPos.x, upPos.y - 2, upPos.z);

        coll = GetComponent<Collider>();
        coll.enabled = false;
        spikeObj.transform.position = downPos;
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
        if (isUp)
        {
            spikeObj.transform.position = downPos;
            coll.enabled = false;
            isUp = false;
        }
        else
        {
            spikeObj.transform.position = upPos;
            coll.enabled = true; 
            isUp = true;
        }
    }
}