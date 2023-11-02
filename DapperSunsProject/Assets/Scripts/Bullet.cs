using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public Rigidbody rb;

    public void Initialize(Vector3 velocity, int bulletDamage)
    {
        damage = bulletDamage;
        rb.velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var target = collision.gameObject.GetComponent<IDamage>();
        if (target != null)
        {
            target.takeDamage(damage);  
        }

        
        Destroy(gameObject);
    }
}
