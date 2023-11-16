using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] bool shouldParent;

    bool colliding;
    int counter;

    void FixedUpdate()
    {
        if (colliding)
        {
            counter++;
            if (counter == 3)
            {
                colliding = false;
                GameManager.instance.playerScript.Unground();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (shouldParent)
            {
                GameManager.instance.playerScript.Ground(transform.parent.transform); 
            }
            else
            {
                GameManager.instance.playerScript.Ground(null);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            colliding = true;
            counter = 0;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            colliding = false;
            GameManager.instance.playerScript.Unground();
        }
    }
}