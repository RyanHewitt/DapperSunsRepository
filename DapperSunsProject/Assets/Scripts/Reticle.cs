using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : Beat
{
    [SerializeField] Image reticleImg;

    bool isHighlighted;

    protected override void DoBeat()
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
