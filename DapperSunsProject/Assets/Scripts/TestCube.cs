using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestCube : MonoBehaviour, IDamage, IBoop
{
    [Range(1, 10)][SerializeField] int HP;
    bool isBig;
    [SerializeField] NavMeshAgent agent;
    Vector3 playerDirection;
    [SerializeField] Rigidbody rb;

    void Update()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        playerDirection = GameManager.instance.player.transform.position - agent.transform.position;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void DoBoop(float force)
    {
        rb.AddForce(-playerDirection * force, ForceMode.Impulse);
    }
}