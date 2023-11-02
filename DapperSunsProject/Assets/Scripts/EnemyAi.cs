using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : Beat, IDamage
{
    [Header("---Components---")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Color flash;
    [SerializeField] Material returnColor;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform rotatePos;


    [Header("---Stats---")]
    [SerializeField] int HP;
    [SerializeField] int PlayerFaceSpeed;

    [Header("---Gun stats---")]
    [SerializeField] GameObject bullet;

    Vector3 playerDirection;
    bool playerInRange;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(playerInRange)
        {
            playerDirection = GameManager.instance.player.transform.position - transform.position;

            Look();

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                FaceTarget();
            }
            agent.SetDestination(GameManager.instance.player.transform.position);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(int Amount)
    {
        HP -= Amount;

        StartCoroutine(FlashColor());

        if(HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void DoBeat()
    {
        if (playerInRange)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
        }
    }
    IEnumerator FlashColor()
    {
        model.material.color = flash;

        yield return new WaitForSeconds(0.1f);

        model.material = returnColor;
    }
    void FaceTarget()
    {
        Quaternion Rotation = Quaternion.LookRotation(playerDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, Rotation, Time.deltaTime * PlayerFaceSpeed);
    }
    void Look()
    {
        Vector3 playerDirection = GameManager.instance.player.transform.position - rotatePos.position;

        float verticalAngle = -Mathf.Atan2(playerDirection.y, playerDirection.magnitude);

        Quaternion verticalRotation = Quaternion.AngleAxis(verticalAngle * Mathf.Rad2Deg, Vector3.right);

        rotatePos.localRotation = verticalRotation;
    }
}
