using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour, IDamage, IBoop
{
    [Header("---Components---")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider enemyCol;
    [SerializeField] protected GameObject outline;
    
    [SerializeField] protected Color flashColor;

    [Header("---Stats---")]
    [SerializeField] protected int HP;
    [SerializeField] protected int PlayerFaceSpeed;
    [SerializeField] protected int maxVerticalAngle;
    [SerializeField] protected int boopMultiplier;

    [Header("---Gun stats---")]
    [SerializeField] protected GameObject bullet;

    [Header("---Audio---")]
    [SerializeField] protected AudioClip ShootAudio;
    [SerializeField] protected AudioClip deathAudio;

    protected Vector3 startPos;
    protected Quaternion startRot;
    protected Vector3 playerDirection;
    protected int startHP;
    protected bool playerInRange;

    protected Renderer baseModel;
    protected Renderer outlineModel;
    protected Material outlineMat;
    protected Color baseColor;
    protected Color baseEmission;

    protected virtual void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;
        GameManager.instance.OnRestartEvent += Restart;

        baseModel = GetComponent<Renderer>();
        outlineModel = outline.GetComponent<Renderer>();

        outlineMat = outline.GetComponent<Renderer>().material;
        baseColor = outlineMat.color;
        baseEmission = outlineMat.GetColor("_EmissionColor");

        startPos = transform.position;
        startRot = transform.rotation;
        startHP = HP;
    }

    protected virtual void Update()
    {
        if (playerInRange && CanSeePlayer())
        {
            playerDirection = GameManager.instance.player.transform.position - transform.position;

            Rotate();

            Move();
        }
        else
        {
            playerInRange = false;
        }
    }
    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (GameManager.instance.player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        RaycastHit hit;
        // Cast a ray from the enemy to the player
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            // Debug line to visualize the ray in the Scene view
            Debug.DrawLine(transform.position, hit.point, Color.red);

            // Check if the raycast hit the player
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        // If the raycast didn't hit anything or didn't hit the player, return false
        return false;
    }


    protected virtual void Restart()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        rb.velocity = Vector3.zero;

        HP = startHP;

        baseModel.enabled = true;
        outlineModel.enabled = true;

        enemyCol.enabled = true;

        playerInRange = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(int amount)
    {
        Damage(amount);
    }

    protected virtual void Damage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            StartCoroutine(Death());
        }
    }

    protected virtual void Move()
    {

    }

    protected IEnumerator Flash()
    {
        outlineMat.color = flashColor;
        outlineMat.SetColor("_EmissionColor", flashColor);

        yield return new WaitForSeconds(0.1f);

        outlineMat.color = baseColor;
        outlineMat.SetColor("_EmissionColor", baseEmission);
    }

    protected virtual IEnumerator Death()
    {
        AudioManager.instance.Play3D(deathAudio, transform.position);

        yield return StartCoroutine(Flash());

        baseModel.enabled = false;
        outlineModel.enabled = false;
        enemyCol.enabled = false;
    }

    void DoBeat()
    {
        BeatAction();
    }

    protected virtual void BeatAction()
    {

    }

    protected virtual void Rotate()
    {

    }

    public void DoBoop(float force, bool slam = false)
    {
        BoopImpulse(force, slam);
    }

    protected virtual void BoopImpulse(float force, bool slam = false)
    {

    }
}