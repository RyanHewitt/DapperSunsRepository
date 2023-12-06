using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DoorToggle : MonoBehaviour
{
    [SerializeField] GameObject Door;
    [SerializeField] GameObject trigger;
    [SerializeField] GameObject target;
    bool turnontoggle;
    private Vector3 startsize;
    [SerializeField] float pulse = 1.15f;
    [SerializeField] float returnspeed = 5f;
    [SerializeField] float EmissiveOn = 2.416924f;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
        turnontoggle = false;
        trigger.SetActive(false);
        Door.GetComponent<Renderer>().material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        Door.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red * EmissiveOn);

        GameManager.instance.OnBeatEvent += DoBeat;

        startsize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!target.GetComponent<Collider>().enabled)
        {
            turnontoggle = true;
        }
        if(turnontoggle == true)
        {
            trigger.SetActive(true);
            Door.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green * EmissiveOn);
            transform.localScale = Vector3.Lerp(transform.localScale, startsize, Time.deltaTime * returnspeed);
        }
    }
    void DoBeat()
    {
        transform.localScale = new Vector3(startsize.x, startsize.y * pulse, startsize.z);
    }
    void Restart()
    {
        turnontoggle = false;
        trigger.SetActive(false);
        Door.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red * EmissiveOn);
    }
}
