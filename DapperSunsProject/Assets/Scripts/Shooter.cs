using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour, IDamage, IBoop
{
    [Header("---Components---")]
    [SerializeField] GameObject outline;
    [SerializeField] Color flashColor;
    [SerializeField] Transform shootPos;
    [SerializeField] Rigidbody rb;

    [Header("---Stats---")]
    [SerializeField] int HP;
    [SerializeField] int PlayerFaceSpeed;
    [SerializeField] int maxVerticalAngle;

    [Header("---Gun stats---")]
    [SerializeField] GameObject bullet;

    Color baseColor;
    Vector3 playerDirection;
    Material outlineMat;
    bool playerInRange;

    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;
        outlineMat = outline.GetComponent<Renderer>().material;
        baseColor = outlineMat.color;
    }

    void Update()
    {
        if(playerInRange)
        {
            playerDirection = GameManager.instance.player.transform.position - transform.position;

            FaceTarget();
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

    public void takeDamage(int Amount, Vector3? knockbackDirection = null)
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

    IEnumerator FlashColor()
    {
        outlineMat.color = flashColor;
        outlineMat.SetColor("_EmissionColor", flashColor);

        yield return new WaitForSeconds(0.1f);

        outlineMat.color = baseColor;
        outlineMat.SetColor("_EmissionColor", baseColor);
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

    public void DoBoop(float force)
    {
        rb.AddForce(-playerDirection * force, ForceMode.Impulse);
    }
}