using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGunTrail : MonoBehaviour
{
    public GameObject _RailGunTrail;
    public float trailInterval;
    public int maxTrailCount;
    public float trailDuration;

    void Start()
    {
        StartCoroutine(CreateTrail());
    }

    private IEnumerator CreateTrail()
    {
        for (int i = 0; i < maxTrailCount; i++)
        {
            // Instantiate the trail
            GameObject trailEffect = Instantiate(_RailGunTrail, transform.position, transform.rotation);

            // Destroy the trail effect after a certain duration
            Destroy(trailEffect, trailDuration);

            // Wait for the specified interval before creating the next trail
            yield return new WaitForSeconds(trailInterval);
        }
        Destroy(this.gameObject);
    }
}

