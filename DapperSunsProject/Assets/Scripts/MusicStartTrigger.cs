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
            AudioManager.instance.playAudio(musicClip);
            GameManager.instance.SyncBeats(bpm);
            gameObject.SetActive(false);
        }
    }
}
