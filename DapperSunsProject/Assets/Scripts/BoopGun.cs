using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoopGun : Beat
{
    [SerializeField] GameObject gunScreen;
    [SerializeField] GameObject gunRing;
    [SerializeField] float pulseSpeed;

    [SerializeField] Color offColor;
    [SerializeField] Color onColor;

    Color currentColor;

    Material ringMat;
    Material screenMat;

    float elapsedTime;

    protected override void Start()
    {
        base.Start();

        ringMat = gunRing.GetComponent<Renderer>().material;
        currentColor = offColor;
        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentColor);

        screenMat = gunScreen.GetComponent<Renderer>().material;
    }

    protected override void Update()
    {
        base.Update();

        elapsedTime += Time.deltaTime * pulseSpeed;
        currentColor = Color.Lerp(currentColor, offColor, elapsedTime);
        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentColor);
    }

    protected override void DoBeat()
    {
        currentColor = onColor;
        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentColor);
        elapsedTime = 0;
    }
}