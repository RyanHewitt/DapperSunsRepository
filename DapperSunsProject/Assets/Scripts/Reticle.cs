using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    [SerializeField] Image reticleImg;

    bool isHighlighted;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;
    }

    void DoBeat()
    {
        if (isHighlighted)
        {
            isHighlighted = !isHighlighted;
            reticleImg.color = Color.black;
        }
        else
        {
            isHighlighted = !isHighlighted;
            reticleImg.color = Color.red;
        }
    }
}
