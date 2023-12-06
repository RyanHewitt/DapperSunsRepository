using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Shooter
{
    public LineRenderer laserLineRenderer;
    public Transform LazerPosition;
    public GameObject lazer;
    private Vector3 startSize;
    [SerializeField] float pulse = 1.15f;
    [SerializeField] float returnSpeed = 5f;
    protected override void Start()
    {
        base.Start();
        lazer.SetActive(true);
        laserLineRenderer.enabled = true;
        startSize = laserLineRenderer.startWidth * Vector3.one;
    }

    protected override void Update()
    {
        base.Update();

        Vector3 aimDirection = LazerPosition.forward;

        Ray ray = new Ray(transform.position, aimDirection);
        RaycastHit hit;

        // Check if the ray hits an object.
        if (Physics.Raycast(ray, out hit))
        {
            // Enable the laser and set its positions.
            laserLineRenderer.SetPosition(0, transform.position);
            laserLineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            laserLineRenderer.SetPosition(0, transform.position);
            laserLineRenderer.SetPosition(1, transform.position + transform.forward * 9999);
        }
        laserLineRenderer.startWidth = Mathf.Lerp(laserLineRenderer.startWidth, startSize.x, Time.deltaTime * returnSpeed);
        laserLineRenderer.endWidth = Mathf.Lerp(laserLineRenderer.endWidth, startSize.x, Time.deltaTime * returnSpeed);

    }

    protected override void Restart()
    {
        base.Restart();
        laserLineRenderer.enabled = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            steps = stepsOriginal;
        }
        base.OnTriggerEnter(other);
    }

    protected override IEnumerator Death()
    {
        laserLineRenderer.enabled = false;
        return base.Death();
    }

    protected override void BeatAction()
    {
        base.BeatAction();
        laserLineRenderer.startWidth = startSize.x * pulse;
        laserLineRenderer.endWidth = startSize.x * pulse;
    }
}