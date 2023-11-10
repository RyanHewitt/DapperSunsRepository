using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;
    public AudioLowPassFilter lowPassFilter;

    void Awake()
    {
        instance = this;
    }

    public void playAudio(AudioClip input)
    {
        audioSource.clip = input;
        audioSource.Play();
    }

    public void playOnce(AudioClip input)
    {
        audioSource.PlayOneShot(input);
    }

    public void Unmuffle()
    {
        StopAllCoroutines();
        StartCoroutine(UnmuffleLerp());
    }

    public void Muffle()
    {
        StopAllCoroutines();
        StartCoroutine(MuffleLerp());
    }

    IEnumerator UnmuffleLerp()
    {
        float timeElapsed = 0f;
        float startValue = lowPassFilter.cutoffFrequency;

        while (timeElapsed < 0.1f)
        {
            lowPassFilter.cutoffFrequency = Mathf.RoundToInt(Mathf.Lerp(startValue, 22000, timeElapsed / 0.1f));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        lowPassFilter.cutoffFrequency = 22000;

        yield break;
    }

    IEnumerator MuffleLerp()
    {
        float timeElapsed = 0f;
        float startValue = lowPassFilter.cutoffFrequency;

        while (timeElapsed < 0.1f)
        {
            lowPassFilter.cutoffFrequency = Mathf.RoundToInt(Mathf.Lerp(startValue, 800, timeElapsed / 0.1f));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        lowPassFilter.cutoffFrequency = 800;

        yield break;
    }
}