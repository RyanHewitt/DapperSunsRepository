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
    [SerializeField] float rotationSpeed = 10f;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
    }

    void Update()
    {
        // Rotate
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
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
