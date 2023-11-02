using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RythmIndicator : Beat
{
    private Vector3 startsize;
    [SerializeField] float pulse = 1.15f;
    [SerializeField] float returnspeed = 5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        startsize = transform.localScale;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        float newScaleY = Mathf.Lerp(transform.localScale.y, startsize.y, Time.deltaTime * returnspeed);
        float newScaleX = Mathf.Lerp(transform.localScale.x, startsize.x, Time.deltaTime * returnspeed);
        transform.localScale = new Vector3(startsize.x, newScaleY, startsize.z);
        transform.localScale = new Vector3(newScaleX, newScaleY, startsize.z);
        transform.localScale = new Vector3(startsize.y, newScaleX, startsize.z);
        transform.localScale = new Vector3(newScaleY, newScaleY, startsize.z);
    }
    protected override void DoBeat()
    {
        transform.localScale = new Vector3(startsize.x, startsize.y * pulse, startsize.z);
    }
}