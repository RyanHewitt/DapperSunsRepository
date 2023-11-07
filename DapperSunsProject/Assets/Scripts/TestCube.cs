using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestCube : Beat, IDamage, IBoop
{
    [Range(1, 10)][SerializeField] int HP;
    bool isBig;
    [SerializeField] NavMeshAgent agent;
    Vector3 playerDirection;
    [SerializeField] Rigidbody rb;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        agent.SetDestination(GameManager.instance.player.transform.position);
        playerDirection = GameManager.instance.player.transform.position - agent.transform.position;
    }

    protected override void DoBeat()
    {
        //if (isBig)
        //{
        //    transform.localScale -= Vector3.one;
        //    isBig = !isBig;
        //}
        //else
        //{
        //    transform.localScale += Vector3.one;
        //    isBig = !isBig;
        //}
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