using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected int damage;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float life = 3;

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
        if (damageable != null && (layerMask & (1 << other.gameObject.layer)) != 0 && !other.GetComponent<EnemyAi>())
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