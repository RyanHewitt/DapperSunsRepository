using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoopBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Renderer model;
    [SerializeField] float bulletSpeed;
    [SerializeField] float life;
    [SerializeField] float scaleFactor;
    [SerializeField] float fadeSpeed;

    Material mat;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;

        rb.velocity = -transform.forward * bulletSpeed;
        transform.localRotation *= Quaternion.Euler(90, 0, 0);
        mat = model.material;

        StartCoroutine(DeathTimer());
    }

    void Update()
    {
        gameObject.transform.localScale += gameObject.transform.localScale * scaleFactor * Time.deltaTime;

        Color color = mat.color;
        color.a -= fadeSpeed * Time.deltaTime;
        mat.color = color;
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