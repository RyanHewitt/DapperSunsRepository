using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider SFXSlider;

    const string MixerMusic = "MusicVolume";
    const string MixerSFX = "SFXVolume";

    // Start is called before the first frame update
    void Awake()
    {
        MusicSlider.onValueChanged.AddListener(MusicVol);
        SFXSlider.onValueChanged.AddListener(SFXVol);
    }

    void MusicVol(float val)
    {
        mixer.SetFloat(MixerMusic, Mathf.Log10(val) * 20);
    }
    
    void SFXVol(float val)
    {
        mixer.SetFloat(MixerMusic, Mathf.Log10(val) * 20);
    }
}
