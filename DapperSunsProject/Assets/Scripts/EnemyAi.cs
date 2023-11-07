using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour, IDamage
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
    [SerializeField] int maxVerticalAngle;

    [Header("---Gun stats---")]
    [SerializeField] GameObject bullet;

    Vector3 playerDirection;
    bool playerInRange;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;

        agent.updateRotation = false;
    }

    void Update()
    {
        if(playerInRange)
        {
            playerDirection = GameManager.instance.player.transform.position - transform.position;

            //LookVert();

            //if (agent.remainingDistance < agent.stoppingDistance)
            //{
            //    FaceTarget();
            //}

            FaceTarget();

            //agent.SetDestination(GameManager.instance.player.transform.position);
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
            gameObject.SetActive(false);
        }
    }

    void DoBeat()
    {
        if (playerInRange)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
        }
    }

    IEnumerator FlashColor() // Change this to outline
    {
        model.material.color = flash;

        yield return new WaitForSeconds(0.1f);

        model.material = returnColor;
    }

    void FaceTarget()
    {
        Vector3 targetDirection = playerDirection;
        if (targetDirection != Vector3.zero)
        {
            // Horizontal rotation (side to side)
            Quaternion horizontalRotation = Quaternion.LookRotation(targetDirection);

            // Vertical rotation (up and down)
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.magnitude) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -maxVerticalAngle, maxVerticalAngle);

            Quaternion verticalRotation = Quaternion.Euler(-angle, 0, 0);

            // Combine both rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, horizontalRotation * verticalRotation, Time.deltaTime * PlayerFaceSpeed);
        }
    }

    void LookVert()
    {
        float verticalAngle = -Mathf.Atan2(playerDirection.y, playerDirection.magnitude);

        Quaternion verticalRotation = Quaternion.AngleAxis(verticalAngle * Mathf.Rad2Deg, Vector3.right);

        transform.rotation = verticalRotation;
    }
}