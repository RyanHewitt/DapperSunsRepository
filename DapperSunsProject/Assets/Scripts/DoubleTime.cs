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
    [SerializeField] float pulseScale = 1.2f;
    [SerializeField] float pulseDuration = 0.5f;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
        StartCoroutine(PulseCoroutine());
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

    IEnumerator PulseCoroutine()
    {
        float beatInterval = 60f / bpm;

        while (true)
        {
            // Grow
            float timer = 0;
            while (timer < pulseDuration)
            {
                float scale = Mathf.Lerp(1f, pulseScale, timer / pulseDuration);
                transform.localScale = new Vector3(scale, scale, scale);
                timer += Time.deltaTime;
                yield return null;
            }

            // Shrink
            timer = 0;
            while (timer < pulseDuration)
            {
                float scale = Mathf.Lerp(pulseScale, 1f, timer / pulseDuration);
                transform.localScale = new Vector3(scale, scale, scale);
                timer += Time.deltaTime;
                yield return null;
            }

           
            yield return new WaitForSeconds(beatInterval - 2 * pulseDuration);
        }
    }
}
