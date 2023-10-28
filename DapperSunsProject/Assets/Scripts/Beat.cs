using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    float bpm;
    float timer;

    protected virtual void Start()
    {
        GameManager.instance.beatObjects.Add(this);
    }

    protected virtual void Update()
    {
        if (Time.time - timer > (60f / bpm))
        {
            timer = Time.time;
            DoBeat();
        }
    }

    public void StartTimer()
    {
        timer = Time.time;
    }

    public void SetBPM(float _bpm)
    {
        bpm = _bpm;
    }

    protected virtual void DoBeat()
    {

    }
}