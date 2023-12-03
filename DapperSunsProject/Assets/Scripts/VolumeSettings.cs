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

    public const string MixerMusic = "MusicVolume";
    public const string MixerSFX = "SFXVolume";
    public const string MixerMaster = "MasterVolume";

    // Start is called before the first frame update
    void Awake()
    {
        MusicSlider.onValueChanged.AddListener(MusicVol);
        SFXSlider.onValueChanged.AddListener(SFXVol);
        MasterSlider.onValueChanged.AddListener(MasterVol);
    }
    private void Start()
    {
        MasterSlider.value = PlayerPrefs.GetFloat(AudioManager.MasterKey, 1f);
        MusicSlider.value = PlayerPrefs.GetFloat(AudioManager.MusicKey, 1f);
        SFXSlider.value = PlayerPrefs.GetFloat(AudioManager.SFXKey, 1f);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat(AudioManager.MasterKey, MasterSlider.value);
        PlayerPrefs.SetFloat(AudioManager.MusicKey, MusicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.SFXKey, SFXSlider.value);
    }

    void MusicVol(float val)
    {
        mixer.SetFloat(MixerMusic, Mathf.Log10(val) * 20);
    }
    
    void SFXVol(float val)
    {
        mixer.SetFloat(MixerSFX, Mathf.Log10(val) * 20);
    }
    void MasterVol(float val)
    {
        mixer.SetFloat(MixerMaster, Mathf.Log10(val) * 20);
    }
}
