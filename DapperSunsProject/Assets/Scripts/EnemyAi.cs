using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour, IDamage, IBoop
{
    [Header("---Components---")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider enemyCol;
    [SerializeField] protected GameObject outline;
    [SerializeField] protected Transform shootPos;
    [SerializeField] protected Color flashColor;
    [SerializeField] protected AudioClip ShootAudio;

    [Header("---Stats---")]
    [SerializeField] protected int HP;
    [SerializeField] protected int PlayerFaceSpeed;
    [SerializeField] protected int maxVerticalAngle;
    [SerializeField] protected int boopMultiplier;

    [Header("---Gun stats---")]
    [SerializeField] protected GameObject bullet;

    protected Vector3 startPos;
    protected Quaternion startRot;
    protected Vector3 playerDirection;
    protected int startHP;
    protected bool playerInRange;

    protected Renderer baseModel;
    protected Renderer outlineModel;
    protected Material outlineMat;
    protected Color baseColor;

    protected virtual void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;
        GameManager.instance.OnRestartEvent += Restart;

        baseModel = GetComponent<Renderer>();
        outlineModel = outline.GetComponent<Renderer>();

        outlineMat = outline.GetComponent<Renderer>().material;
        baseColor = outlineMat.color;

        startPos = transform.position;
        startRot = transform.rotation;
        startHP = HP;
    }

    protected virtual void Update()
    {

        if (playerInRange)
        {
            playerDirection = GameManager.instance.player.transform.position - transform.position;

            Rotate();

            Move();
        }
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

    protected virtual IEnumerator Death()
    {
        yield break;
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

    public void DoBoop(float force)
    {
        BoopImpulse(force);
    }

    protected virtual void BoopImpulse(float force)
    {

    }
}