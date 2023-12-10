using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class FragilePlatform : MonoBehaviour, IBoop
{
    [SerializeField] Collider trigger;
    [SerializeField] Collider col;
    [SerializeField] Renderer[] models;
    [SerializeField] GameObject[] outlines;

    [SerializeField] AudioClip BreakSound;
    [SerializeField] AudioClip countSound;

    [SerializeField] Color flashColor;

    [SerializeField] private float breakTime = 3f;
    [SerializeField] int countdown;

    Color baseColor;
    Color baseEmission;

    bool startCountdown;
    int counter;
    int breakCount;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
        GameManager.instance.OnBeatEvent += BeatAction;

        baseColor = outlines[0].GetComponent<Renderer>().material.color;
        baseEmission = outlines[0].GetComponent<Renderer>().material.GetColor("_EmissionColor");
    }

    public void DoBoop(Vector3 origin, float force, bool slam = false)
    {
        if (col.enabled)
        {
            StartCoroutine(Break());
        }
    }

    IEnumerator Break()
    {
        AudioManager.instance.Play3D(BreakSound, transform.position);

        breakCount++;
        col.enabled = false;
        trigger.enabled = false;

        foreach (Renderer model in models)
        {
            model.enabled = false;
        }

        foreach (GameObject outline in outlines)
        {
            outline.SetActive(false); 
        }

        yield return new WaitForSeconds(breakTime);

        if (breakCount == 1)
        {
            Restart();
            breakCount--;
        }
        else
        {
            breakCount--;
        }
    }

    IEnumerator Flash()
    {
        foreach (GameObject outline in outlines)
        {
            Material outlineMat = outline.GetComponent<Renderer>().material;
            
            outlineMat.color = flashColor;
            outlineMat.SetColor("_EmissionColor", flashColor);
        }

        yield return new WaitForSeconds(0.1f);

        foreach (GameObject outline in outlines)
        {
            Material outlineMat = outline.GetComponent<Renderer>().material;
            outlineMat.color = baseColor;
            outlineMat.SetColor("_EmissionColor", baseEmission);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startCountdown = true; 
        }
    }

    void BeatAction()
    {
        if (startCountdown)
        {
            counter++;
            AudioManager.instance.Play3D(countSound, transform.position);
            StartCoroutine(Flash());

            if (counter >= countdown)
            {
                StartCoroutine(Break());

                startCountdown = false;
                counter = 0;
            }
        }
    }

    void Restart()
    {
        startCountdown = false;
        col.enabled = true;
        trigger.enabled = true;

        foreach (Renderer model in models)
        {
            model.enabled = true;
        }

        foreach (GameObject outline in outlines)
        {
            outline.SetActive(true);
            Material outlineMat = outline.GetComponent<Renderer>().material;
            outlineMat.color = baseColor;
            outlineMat.SetColor("_EmissionColor", baseEmission);
        }

        counter = 0;
    }
}