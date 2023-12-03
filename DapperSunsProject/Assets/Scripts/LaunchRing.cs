using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchRing : MonoBehaviour
{
    [SerializeField] float force;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IBoop boopable = other.GetComponent<IBoop>();
        if (boopable != null )
        {
            boopable.DoBoop(transform.position + (transform.forward * -10), force);
        }
    }
}