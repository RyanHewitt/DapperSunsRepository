using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoopGun : Beat
{
    [SerializeField] GameObject gunScreen;
    [SerializeField] GameObject gunRing;
    [SerializeField] float pulseSpeed;
    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;

    Material screenMat;
    Material ringMat;

    protected override void Start()
    {
        base.Start();

        gunScreen.GetComponent<MeshRenderer>().material = screenMat;
        screenMat = offMaterial;
        screenMat.color = Color.white;

        //gunScreen.GetComponent<MeshRenderer>().material = offMaterial;

        ringMat = gunRing.GetComponent<MeshRenderer>().material;
        ringMat = offMaterial;
        ringMat.color = Color.white;
    }

    protected override void Update()
    {
        base.Update();

        ringMat.Lerp(onMaterial, offMaterial, pulseSpeed * Time.deltaTime);
    }

    protected override void DoBeat()
    {
        ringMat = onMaterial;
    }
}