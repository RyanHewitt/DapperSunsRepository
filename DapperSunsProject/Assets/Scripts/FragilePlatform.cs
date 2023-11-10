using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FragilePlatform : MonoBehaviour, IBoop
{
    [SerializeField] private float _breakTime = 3f;
    [SerializeField] Collider col;
    [SerializeField] Renderer model;
    [SerializeField] GameObject outline;

    int breakCount;

    public float breakTime
    {
        get { return _breakTime; }
        set { _breakTime = value; }
    }

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
    }

    public void DoBoop(float force)
    {
        StartCoroutine(Break());
    }

    IEnumerator Break()
    {
        breakCount++;
        col.enabled = false;
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

    void Restart()
    {
        col.enabled = true;
        model.enabled = true;
        outline.SetActive(true);
    }
}