using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Ground");
            if (transform.parent != null)
            {
                GameManager.instance.playerScript.Ground(transform.parent.transform); 
            }
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
            Debug.Log("Unground");
            GameManager.instance.playerScript.Unground();
        }
    }
}
