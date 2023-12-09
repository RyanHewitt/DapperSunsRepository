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
            if (PlayerPrefs.GetString("ogSongClip") == musicClip.name)
            {
                float time = PlayerPrefs.GetFloat("ogSongTime");
                GameManager.instance.SyncBeats(bpm, time);
            }
            else
            {
                GameManager.instance.SyncBeats(bpm);
            }
            gameObject.SetActive(false);
        }
    }
}