using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoopGun : MonoBehaviour
{
    [SerializeField] GameObject gunScreen;
    [SerializeField] GameObject gunRing;
    [SerializeField] GameObject gunSpeaker;
    [SerializeField] float pulseSpeed;

    [SerializeField] Color offColor;

    Color onColor;
    Color onEmission;
    Color currentColor;
    Color currentEmission;

    Material ringMat;
    Material screenMat;

    float elapsedTime;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        ringMat = gunRing.GetComponent<Renderer>().material;

        onColor = ringMat.color;
        onEmission = ringMat.GetColor("_EmissionColor");

        currentColor = onColor;
        currentEmission = onEmission;

        screenMat = gunScreen.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * pulseSpeed;

        currentColor = Color.Lerp(currentColor, offColor, elapsedTime);
        ringMat.color = currentColor;

        currentEmission = Color.Lerp(currentEmission, offColor, elapsedTime);
        ringMat.SetColor("_EmissionColor", currentColor);

        gunSpeaker.transform.localScale = Vector3.Lerp(gunSpeaker.transform.localScale, Vector3.one, elapsedTime);

        float t = GameManager.instance.beatTime - 0.25f;
        t -= Mathf.Floor(t);
        screenMat.mainTextureOffset = Vector2.Lerp(Vector2.zero, Vector2.down * 0.95f, t);
    }

    void DoBeat()
    {
        elapsedTime = 0;

        currentColor = onColor;
        currentEmission = onEmission;

        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentEmission);

        gunSpeaker.transform.localScale *= 1.1f;
    }
}