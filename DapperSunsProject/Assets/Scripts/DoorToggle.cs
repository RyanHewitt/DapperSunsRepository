using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorToggle : MonoBehaviour
{
    [SerializeField] GameObject Door;
    bool turnontoggle;
    private Vector3 startsize;
    [SerializeField] float pulse = 1.15f;
    [SerializeField] float returnspeed = 5f;
    [SerializeField] float EmissiveOn = 2.416924f;
    // Start is called before the first frame update
    void Start()
    {
        Door.GetComponentInChildren<Collider>().enabled = false;
        turnontoggle = false;
        Door.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        Door.GetComponent<Renderer>().material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        Door.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green * 0);

        GameManager.instance.OnBeatEvent += DoBeat;

        startsize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(turnontoggle == true)
        {
            Door.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green * EmissiveOn);
            transform.localScale = Vector3.Lerp(transform.localScale, startsize, Time.deltaTime * returnspeed);
            Door.GetComponentInChildren<Collider>().enabled = true;
        }
    }
    void DoBeat()
    {
        transform.localScale = new Vector3(startsize.x, startsize.y * pulse, startsize.z);
    }
}
