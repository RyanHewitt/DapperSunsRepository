using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    [SerializeField] float bpm;
    
    float timer;

    protected virtual void Start()
    {
        StartMusic();
    }

    protected virtual void Update()
    {
        if (Time.time - timer > (60f / bpm))
        {
            timer = Time.time;
            DoBeat();
        }
    }

    void StartMusic()
    {
        timer = Time.time;
        // Start Music Here
    }

    protected virtual void DoBeat()
    {

    }
}