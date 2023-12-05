using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RythmIndicator : MonoBehaviour
{
    [SerializeField] float pulse = 1.15f;
    [SerializeField] float returnspeed = 5f;
    [SerializeField] private Vector2 modVector = Vector2.one;

    Vector3 startsize;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        startsize = transform.localScale;
    }

    void Update()
    {
        float newScaleY = Mathf.Lerp(transform.localScale.y, startsize.y, Time.deltaTime * returnspeed);
        float newScaleX = Mathf.Lerp(transform.localScale.x, startsize.x, Time.deltaTime * returnspeed);

        transform.localScale = new Vector3(newScaleX, newScaleY, startsize.z);
    }

    void DoBeat()
    {
        transform.localScale = new Vector3(startsize.x * pulse * modVector.x, startsize.y * pulse * modVector.y, startsize.z);
    }
}