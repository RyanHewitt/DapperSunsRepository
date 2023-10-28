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

    protected virtual void DoBeat()
    {

    }
}