using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;

    public bool isPlaying;

    void Awake()
    {
        instance = this;
        isPlaying = false;
    }

    public void playAudio(AudioClip input) // Loops
    {
        audioSource.clip = input;
        audioSource.Play();
        isPlaying = true;
    }

    public void playOnce(AudioClip input) // No loop
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
}