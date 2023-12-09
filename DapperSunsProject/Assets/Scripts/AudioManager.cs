using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioSource MusicSource;
    public AudioLowPassFilter lowPassFilter;
    public bool musicPlaying;
    [SerializeField] AudioMixer Mixer;
    [SerializeField] AudioMixerGroup SFX;
    [SerializeField] AudioMixerGroup Music;

    void Awake()
    {
        instance = this;
        LoadVolume();
    }

    public void playAudio(AudioClip input)
    {
        audioSource.clip = input;
        audioSource.Play();
    }

    public void Play3D(AudioClip clip, Vector3 pos, float pitch = 1f, float spatial = 1f, float vol = 1f)
    {
        GameObject audioSourceObject = new GameObject("3DAudio");
        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFX;
        audioSource.pitch = pitch;
        audioSource.volume = vol;

        audioSourceObject.transform.position = pos;
        audioSource.clip = clip;
        audioSource.spatialBlend = spatial;
        audioSource.Play();

        Destroy(audioSourceObject, clip.length);
    }

    public void playOnce(AudioClip input)
    {
        audioSource.outputAudioMixerGroup = SFX;
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

    public void ChangeSong(AudioClip newSong, float time = 0f)
    {
        MusicSource.outputAudioMixerGroup = Music;
        MusicSource.Stop();
        MusicSource.clip = newSong;
        MusicSource.time = time;
        MusicSource.Play();
        musicPlaying = true;
    }

    void LoadVolume()
    {
        float MasterVolume = PlayerPrefs.GetFloat(VolumeSettings.MixerMaster, 1f);
        float MusicVolume = PlayerPrefs.GetFloat(VolumeSettings.MixerMusic, 1f);
        float SFXVolume = PlayerPrefs.GetFloat(VolumeSettings.MixerSFX, 1f);

        Mixer.SetFloat(VolumeSettings.MixerMaster, Mathf.Log10(MasterVolume) * 20);
        Mixer.SetFloat(VolumeSettings.MixerMusic, Mathf.Log10(MusicVolume) * 20);
        Mixer.SetFloat(VolumeSettings.MixerSFX, Mathf.Log10(SFXVolume) * 20);
    }
}