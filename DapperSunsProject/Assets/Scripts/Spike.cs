using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : Beat
{
    [SerializeField] int SpikeDMG;
    private Vector3 startsize;
    [SerializeField] float pulse = 1.15f;
    [SerializeField] float returnspeed = 5f;
    [SerializeField] Color down;
    [SerializeField] Color up;

    protected override void Start()
    {
        base.Start();
        startsize = transform.localScale;
    }

    protected override void Update()
    {
        base.Update();
        float newScaleY = Mathf.Lerp(transform.localScale.y, startsize.y, Time.deltaTime * returnspeed);
        transform.localScale = new Vector3(startsize.x, newScaleY, startsize.z);
    }
    private void OnTriggerEnter(Collider other)
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
        transform.localScale = new Vector3(startsize.x, startsize.y * pulse, startsize.z);
    }
}