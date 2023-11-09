using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoopBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float bulletSpeed;
    [SerializeField] float life;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
        rb.velocity = -transform.forward * bulletSpeed;
        transform.localRotation *= Quaternion.Euler(90, 0, 0);
        StartCoroutine(DeathTimer());
    }

    void Update()
    {
        //gameObject.transform.localScale *= 2f * Time.deltaTime;
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(life);
        Restart();
    }

    void Restart()
    {
        if (gameObject != null)
        {
            GameManager.instance.OnRestartEvent -= Restart;
            Destroy(gameObject);
        }
    }
}