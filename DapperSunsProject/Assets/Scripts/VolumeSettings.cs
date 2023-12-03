using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider SFXSlider;

    public const string MixerMaster = "MasterVolume";
    public const string MixerMusic = "MusicVolume";
    public const string MixerSFX = "SFXVolume";

    void Awake()
    {
        MusicSlider.onValueChanged.AddListener(MusicVol);
        SFXSlider.onValueChanged.AddListener(SFXVol);
        MasterSlider.onValueChanged.AddListener(MasterVol);
    }

    void Start()
    {
        MasterSlider.value = PlayerPrefs.GetFloat(MixerMaster, 1f);
        MusicSlider.value = PlayerPrefs.GetFloat(MixerMusic, 1f);
        SFXSlider.value = PlayerPrefs.GetFloat(MixerSFX, 1f);
    }

    void MasterVol(float val)
    {
        PlayerPrefs.SetFloat(MixerMaster, MasterSlider.value);
        mixer.SetFloat(MixerMaster, Mathf.Log10(val) * 20);
    }

    void MusicVol(float val)
    {
        PlayerPrefs.SetFloat(MixerMusic, MusicSlider.value);
        mixer.SetFloat(MixerMusic, Mathf.Log10(val) * 20);
    }
    
    void SFXVol(float val)
    {
        PlayerPrefs.SetFloat(MixerSFX, SFXSlider.value);
        mixer.SetFloat(MixerSFX, Mathf.Log10(val) * 20);
    }
}
