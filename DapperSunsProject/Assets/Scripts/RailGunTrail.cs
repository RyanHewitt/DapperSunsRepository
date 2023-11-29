using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RailGunTrail : Bullet
{
    public GameObject _RailGunTrail;
    public float trailInterval;
    public int maxTrailCount;
    public float trailDuration;
    private List<GameObject> Trail = new List<GameObject>();
    bool _is;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if(!_is)
        {
            StartCoroutine(CreateTrail());
        }
    }

    protected override void Restart()
    {
        foreach(var trail in Trail)
        {
            Destroy(trail);
        }
        base.Restart();
    }
    private IEnumerator CreateTrail()
    {
        _is = true;
        for (int i = 0; i < maxTrailCount; i++)
        {
            // Instantiate the trail
            GameObject trailEffect = Instantiate(_RailGunTrail, transform.position, transform.rotation);
            Trail.Add(trailEffect);
            // Destroy the trail effect after a certain duration
            Destroy(trailEffect, trailDuration);

            // Wait for the specified interval before creating the next trail
            yield return new WaitForSeconds(trailInterval);
        }
        _is = false;
    }
}

