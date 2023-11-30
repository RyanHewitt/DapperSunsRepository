using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTime : MonoBehaviour
{
    [SerializeField] public GameManager gameManager; 
    [SerializeField] public AudioClip doubleTimeSong;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.ActivateDoubleTimePowerUp(doubleTimeSong);
            Destroy(gameObject);
        }
    }
}
