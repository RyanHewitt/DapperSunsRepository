using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Beat, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform shootPos;

    [Header("----- Player Stats -----")]
    [Range(1, 20)][SerializeField] float playerSpeed;
    [Range(1, 50)][SerializeField] float jumpHeight;
    [Range(-10, 50)][SerializeField] float gravityValue;
    [Range(0, 1)][SerializeField] float friction;

    [Header("----- Gun Stats -----")]
    [SerializeField] int boopDist;
    [SerializeField] int boopForce;
    [SerializeField] GameObject boopCard;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip blastSFX;
    [SerializeField] AudioClip blastPenaltySFX;
    [SerializeField] AudioClip shootSFX;

    Vector3 move;
    Vector3 playerVelocity;
    bool groundedPlayer;
    bool canBoop = false;
    bool hitBeat = false;
    bool hitPenalty = false;
    int HP = 1;

    protected override void Start()
    {
        base.Start();

        GameManager.instance.playerDead = false;
        spawnPlayer();
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time - timer < ((60f / bpm) * 0.25f) || Time.time - timer > ((60f / bpm) * 0.75f))
        {
            canBoop = true;
        }
        else
        {
            if (canBoop)
            {
                hitPenalty = false;
            }
            canBoop = false;
            hitBeat = false;
        }

        if (Input.GetButtonDown("Shoot") && GameManager.instance.menuActive == null)
        {
            if (!hitPenalty)
            {
                if (canBoop && !hitBeat)
                {
                    hitBeat = true;
                    Boop();
                }
                else
                {
                    hitPenalty = true;
                    BoopPenalty();
                }
            }
        }

        MovePlayer();
    }

    void MovePlayer()
    {
        float frictionForce;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.distance < 2)
            {
                groundedPlayer = true;
            }
            else
            {
                groundedPlayer = false;
            }
        }

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            frictionForce = friction;
        }
        else
        {
            frictionForce = friction / 2;
        }

        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        playerVelocity.x *= friction;
        playerVelocity.z *= friction;

        controller.Move((move * Time.deltaTime * playerSpeed) + (playerVelocity * Time.deltaTime));
    }

    void Boop()
    {
        AudioManager.instance.playOnce(blastSFX);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, boopDist))
        {
            Vector3 launchDirection = hit.normal;
            playerVelocity.x += launchDirection.x * boopForce * 2;
            playerVelocity.y += launchDirection.y * boopForce;
            playerVelocity.z += launchDirection.z * boopForce * 2;

            if (!hit.collider.isTrigger)
            {
                IBoop boopable = hit.collider.GetComponent<IBoop>();

                if (hit.transform != transform && boopable != null)
                {
                    boopable.DoBoop(boopForce);
                }
            }
        }

        Instantiate(boopCard, shootPos.position, shootPos.rotation);
    }

    void BoopPenalty()
    {
        AudioManager.instance.playOnce(blastPenaltySFX);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();

        if (HP <= 0)
        {
            GameManager.instance.playerDead = true;
            GameManager.instance.popupLose();
        }
    }

    public void spawnPlayer()
    {
        controller.enabled = false;

        GameObject spawn = GameManager.instance.GetPlayerSpawn();
        transform.position = spawn.transform.position;
        transform.rotation = spawn.transform .rotation;

        controller.enabled = true;

        HP = 1;
        updatePlayerUI();
    }

    public void updatePlayerUI()
    {

    }

    protected override void DoBeat()
    {
        
    }
}