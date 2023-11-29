using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] Rigidbody rb;
    [SerializeField] float bulletSpeed;
    [SerializeField] float life = 3;

    protected virtual void Start()
    {
        if (gameObject != null)
        {
            GameManager.instance.OnRestartEvent += Restart;
            rb.velocity = transform.forward * bulletSpeed;
            StartCoroutine(DeathTimer());
        }
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(life);
        Restart();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.takeDamage(damage);
        }

        Restart();
    }

    protected virtual void Restart()
    {
        if (gameObject != null)
        {
            GameManager.instance.OnRestartEvent -= Restart;
            Destroy(gameObject);
        }
    }
}