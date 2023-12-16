using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour, IDamage, IBoop
{
    [Header("---Movement---")]
    [SerializeField] protected float stopDistance = 2.0f;
    [SerializeField] protected bool parented;
    [SerializeField] protected bool canBoop;

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

    protected Transform parentTransform;
    protected Vector3 startPos;
    protected Quaternion startRot;
    protected Vector3 playerDirection;
    protected int startHP;
    protected bool playerInRange;
    protected bool wasParented;

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

        startRot = transform.rotation;

        if (!parented)
        {
            startPos = transform.position;
        }
        else
        {
            parentTransform = transform.parent;
            startPos = transform.localPosition;
        }
        
        startHP = HP;
    }

    protected virtual void Update()
    {
        if (playerInRange)
        {
            if (CheckLineOfSight(GameManager.instance.player.transform))
            {
                playerDirection = GameManager.instance.player.transform.position - transform.position;
                Rotate();
                Move();
            }
        }
    }

    protected virtual void Restart()
    {
        StopAllCoroutines();

        if (!parented)
        {
            transform.position = startPos;
        }
        else
        {
            gameObject.transform.parent = parentTransform;
            transform.position = parentTransform.position + parentTransform.TransformDirection(startPos);
            rb.useGravity = false;
        }

        transform.rotation = startRot;

        rb.velocity = Vector3.zero;

        HP = startHP;

        baseModel.enabled = true;
        outlineModel.enabled = true;

        enemyCol.enabled = true;

        playerInRange = false;

        outlineMat.color = baseColor;
        outlineMat.SetColor("_EmissionColor", baseEmission);
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
        AudioManager.instance.Play3D(deathAudio, transform.position, 1f, 0.75f);

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

    public void DoBoop(Vector3 origin, float force, bool slam = false)
    {
        if (parented && canBoop)
        {
            gameObject.transform.parent = null;
            rb.useGravity = true;
        }
        BoopImpulse(origin, force, slam);
    }

    protected virtual void BoopImpulse(Vector3 origin, float force, bool slam = false)
    {

    }

    protected bool CheckLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = target.position - transform.position;
        int layerMask = LayerMask.GetMask("Default", "Player");

        if (Physics.Raycast(transform.position, direction.normalized, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform == target)
            {
                return true;
            }
        }
        return false;
    }
}
