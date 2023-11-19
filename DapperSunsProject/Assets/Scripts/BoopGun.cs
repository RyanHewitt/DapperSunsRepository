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

        screenMat = gunScreen.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * pulseSpeed;

        currentColor = Color.Lerp(currentColor, offColor, elapsedTime);
        ringMat.color = currentColor;
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
        ringMat.color = currentColor;
        ringMat.SetColor("_EmissionColor", currentColor);

        gunSpeaker.transform.localScale *= 1.1f;
    }
}