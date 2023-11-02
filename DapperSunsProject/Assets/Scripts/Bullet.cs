using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        var target = collision.gameObject.GetComponent<IDamage>();
        if (target != null)
        {
            target.takeDamage(damage);
        }

        // Optionally, destroy the bullet on collision
        Destroy(gameObject);
    }
}