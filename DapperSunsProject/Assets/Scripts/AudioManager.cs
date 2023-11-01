using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioSource audioSource;

    bool isPlaying;

    void Awake()
    {
        instance = this;
        isPlaying = false;
    }

    public void playAudio(AudioClip input)
    {
        audioSource.clip = input;
        audioSource.Play();
        isPlaying = true;
    }

    public void pauseUnpauseAudio()
    {
        if (isPlaying)
        {
            audioSource.Pause();
            isPlaying = !isPlaying;
        }
        else
        {
            audioSource.UnPause();
            isPlaying = !isPlaying;
        }
    }    
}