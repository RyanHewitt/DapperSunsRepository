using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    private Vector3 startsize;
    [SerializeField] float pulse = 1.15f;
    [SerializeField] float returnspeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        startsize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, startsize, Time.deltaTime * returnspeed);
    }
    void DoBeat()
    {
        transform.localScale = new Vector3(startsize.x, startsize.y * pulse, startsize.z);
    }
}
