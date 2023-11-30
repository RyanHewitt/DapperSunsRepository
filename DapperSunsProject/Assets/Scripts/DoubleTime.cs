using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTime : MonoBehaviour
{
    [SerializeField] private GameManager gameManager; 
    [SerializeField] private AudioClip doubleTimeSong;
    int bpm;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.ActivateDoubleTimePowerUp(doubleTimeSong);
            Destroy(gameObject);
        }
    }
}
