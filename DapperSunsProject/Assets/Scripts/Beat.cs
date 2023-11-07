using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    [Header("----- Beat -----")]
    [SerializeField]
    private float _steps = 1f;

    public float steps
    {
        get { return _steps; }
        set { _steps = value; }
    }

    protected float bpm;
    protected float timer;

    protected virtual void Start()
    {
        //GameManager.instance.beatObjects.Add(this);
    }

    protected virtual void Update()
    {
        if (Time.time - timer > ((60f / bpm) / steps))
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