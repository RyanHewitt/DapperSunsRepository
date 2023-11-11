using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Shooter
{
    public LineRenderer laserLineRenderer;
    public Transform _sniper;

    protected void Update()
    {
        base.Update();
        Vector3 aimDirection = _sniper.forward;

        Ray ray = new Ray(transform.position, aimDirection);
        RaycastHit hit;

        // Check if the ray hits an object.
        if (Physics.Raycast(ray, out hit))
        {
            // Enable the laser and set its positions.
            laserLineRenderer.enabled = true;
            laserLineRenderer.SetPosition(0, transform.position);
            laserLineRenderer.SetPosition(1, hit.point);
            //Debug.Log("Ray hit: " + hit.collider.gameObject.name);
        }
        else
        {
            // Disable the laser if it doesn't hit anything.
            laserLineRenderer.enabled = false;
        }
    }
}