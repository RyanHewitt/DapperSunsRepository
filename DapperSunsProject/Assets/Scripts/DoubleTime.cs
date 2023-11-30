using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTime : MonoBehaviour
{
    [SerializeField] AudioClip doubleTimeSong;
    [SerializeField] Renderer model;
    [SerializeField] Collider col;
    [SerializeField] float duration;
    [SerializeField] float bpm;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.ActivateDoubleTimePowerUp(doubleTimeSong, bpm, duration);
            model.enabled = false;
            col.enabled = false;
        }
    }

    void Restart()
    {
        model.enabled = true;
        col.enabled = true;
    }
}
