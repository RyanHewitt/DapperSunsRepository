using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStartTrigger : MonoBehaviour
{
    [SerializeField] AudioClip musicClip;

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            AudioManager.instance.playAudio(musicClip);
        }
    }
}
