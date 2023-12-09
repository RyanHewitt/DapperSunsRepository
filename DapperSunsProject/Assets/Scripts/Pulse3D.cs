using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse3D : MonoBehaviour
{
    [SerializeField] private Vector3 pulseVector = Vector3.one;
    [SerializeField] float returnspeed = 5f;
    [SerializeField] int steps;
    [SerializeField] bool reverseScale;

    Vector3 startSize;
    int currentStep = 0;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        startSize = transform.localScale;
    }

    void Update()
    {
        if (!reverseScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startSize, returnspeed * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startSize + pulseVector, returnspeed * Time.deltaTime);
        }
    }

    void DoBeat()
    {
        if (currentStep >= steps)
        {
            currentStep = 0;
            if (!reverseScale)
            {
                transform.localScale = startSize + pulseVector;
            }
            else
            {
                transform.localScale = startSize;
            }
        }
        else
        {
            currentStep++;
        }
    }
}
