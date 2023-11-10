using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;
    public AudioLowPassFilter lowPassFilter;

    public bool isPlaying;

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

    public void playOnce(AudioClip input)
    {
        audioSource.PlayOneShot(input);
    }

    public void pauseUnpauseAudio()
    {
        if (audioSource.clip)
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

    public void Muffle()
    {
        //lowPassFilter.cutoffFrequency
    }
}