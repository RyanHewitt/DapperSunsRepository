using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class FragilePlatform : MonoBehaviour, IBoop
{
    [SerializeField] Collider trigger;
    [SerializeField] Collider col;
    [SerializeField] Renderer model;
    [SerializeField] GameObject outline;
    [SerializeField] GameObject[] connectedPlats;
    [SerializeField] float _breakTime = 3f;
    FragilePlatform[] FlashingOutlines;

    int breakCount;

    [SerializeField] protected Color flashColor;
    protected Material outlineMat;
    protected Color baseColor;
    protected Color baseEmission;
    [SerializeField] int countdown;
    [SerializeField] AudioClip BreakSound;
    [SerializeField] AudioClip countSound;

    bool startCountdown;
    int counter;

    public float breakTime
    {
        get { return _breakTime; }
        set { _breakTime = value; }
    }

    void Start()
    {
        FlashingOutlines = new FragilePlatform[connectedPlats.Length];
        for (int i = 0; i < connectedPlats.Length; i++)
        {
            FlashingOutlines[i] = connectedPlats[i].GetComponent<FragilePlatform>();
        }
        GameManager.instance.OnRestartEvent += Restart;
        GameManager.instance.OnBeatEvent += BeatAction;

        outlineMat = outline.GetComponent<Renderer>().material;
        baseColor = outlineMat.color;
        baseEmission = outlineMat.GetColor("_EmissionColor");
    }

    public void DoBoop(Vector3 origin, float force, bool slam = false)
    {
        if (col.enabled)
        {
            StartCoroutine(Break());

            if (connectedPlats.Length > 0)
            {
                foreach (GameObject obj in connectedPlats)
                {
                    IBoop boopable = obj.GetComponent<IBoop>();
                    if (boopable != null)
                    {
                        boopable.DoBoop(Vector3.zero, force);
                    }
                }
            }
        }
    }

    IEnumerator Break()
    {
        breakCount++;
        col.enabled = false;
        trigger.enabled = false;
        model.enabled = false;
        outline.SetActive(false);

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
    protected IEnumerator Flash()
    {
        outlineMat.color = flashColor;
        outlineMat.SetColor("_EmissionColor", flashColor);

        yield return new WaitForSeconds(0.1f);

        outlineMat.color = baseColor;
        outlineMat.SetColor("_EmissionColor", baseEmission);
    }
    public void FlashCall()
    {
        StartCoroutine(Flash());
    }
    private void OnTriggerEnter(Collider other)
    {
        startCountdown = true;
    }
    void BeatAction()
    {
        if (startCountdown)
        {
            counter++;
            AudioManager.instance.Play3D(countSound, transform.position);
            FlashCall();
            foreach (var obj in FlashingOutlines)
            {
                obj.FlashCall();
            }

            if (counter >= countdown)
            {
                StartCoroutine(Break());

                if (connectedPlats.Length > 0)
                {
                    AudioManager.instance.Play3D(BreakSound, transform.position);
                    foreach (GameObject obj in connectedPlats)
                    {
                        IBoop boopable = obj.GetComponent<IBoop>();
                        if (boopable != null)
                        {
                            boopable.DoBoop(Vector3.zero, 1);
                            startCountdown = false;
                            counter = 0;
                        }
                    }
                }
            }
        }
    }

    void Restart()
    {
        startCountdown = false;
        col.enabled = true;
        trigger.enabled = true;
        model.enabled = true;
        outline.SetActive(true);
        counter = 0;
    }
}