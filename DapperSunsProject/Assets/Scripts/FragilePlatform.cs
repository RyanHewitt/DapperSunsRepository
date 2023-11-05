using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FragilePlatform : MonoBehaviour, IBoop
{
    [SerializeField] private float _breakTime = 3f;

    public float breakTime
    {
        get { return _breakTime; }
        set { _breakTime = value; }
    }

    public void DoBoop(float force)
    {
        StartCoroutine(Break());
    }

    IEnumerator Break()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(breakTime);
        gameObject.SetActive(true);
    }
}