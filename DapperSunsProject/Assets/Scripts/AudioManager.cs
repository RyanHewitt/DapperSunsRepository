using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void Play3D(AudioClip clip, Vector3 pos)
    {
        GameObject audioSourceObject = new GameObject("3DAudio");
        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();

        audioSourceObject.transform.position = pos;
        audioSource.clip = clip;
        audioSource.spatialBlend = 1.0f;
        audioSource.Play();

        Destroy(audioSourceObject, clip.length);
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

    public void ChangeSong(AudioClip newSong)
    {
        audioSource.Stop();
        audioSource.clip = newSong;
        audioSource.Play();
    }
}