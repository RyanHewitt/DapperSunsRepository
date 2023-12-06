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
        if (shouldParent && colliding)
        {
            counter++;
            if (counter == 3)
            {
                colliding = false;
                GameManager.instance.playerScript.UnparentMovement(transform);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (shouldParent)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                GameManager.instance.playerScript.ParentMovement(transform);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (shouldParent)
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
    }

    void OnTriggerExit(Collider other)
    {
        if (shouldParent)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                colliding = false;
                GameManager.instance.playerScript.UnparentMovement(transform);
            }
        }
    }
}