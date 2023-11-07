using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStartTrigger : MonoBehaviour
{
    [SerializeField] AudioClip musicClip;
    [SerializeField] float bpm;

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            GameManager.instance.audioClip = musicClip;
            GameManager.instance.SyncBeats(bpm);
            gameObject.SetActive(false);
        }
    }
}
