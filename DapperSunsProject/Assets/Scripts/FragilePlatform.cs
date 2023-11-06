using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FragilePlatform : MonoBehaviour, IBoop
{
    [SerializeField] private float _breakTime = 3f;
    [SerializeField] Collider col;
    [SerializeField] Renderer model;

    Renderer childModel;

    public float breakTime
    {
        get { return _breakTime; }
        set { _breakTime = value; }
    }

    void Start()
    {
        childModel = GetComponentsInChildren<Renderer>()[1];
    }

    public void DoBoop(float force)
    {
        StartCoroutine(Break());
    }

    IEnumerator Break()
    {
        col.enabled = false;
        model.enabled = false;
        childModel.enabled = false;

        yield return new WaitForSeconds(breakTime);

        col.enabled = true;
        model.enabled = true;
        childModel.enabled = true;
    }
}