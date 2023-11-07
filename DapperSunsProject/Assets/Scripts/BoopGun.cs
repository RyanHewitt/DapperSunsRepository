using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoopGun : MonoBehaviour
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

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        ringMat = gunRing.GetComponent<Renderer>().material;
        currentColor = offColor;
        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentColor);

        screenMat = gunScreen.GetComponent<Renderer>().material;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * pulseSpeed;
        currentColor = Color.Lerp(currentColor, offColor, elapsedTime);
        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentColor);
    }

    void DoBeat()
    {
        currentColor = onColor;
        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentColor);
        elapsedTime = 0;
    }
}