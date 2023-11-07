using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform shootPos;

    [Header("----- Player Stats -----")]
    [Range(1, 20)][SerializeField] float playerSpeed;
    [Range(1, 50)][SerializeField] float jumpHeight;
    [Range(-10, 50)][SerializeField] float gravityValue;
    [Range(0, 1)][SerializeField] float friction;

    [Header("----- Dash Stats -----")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 2f;

    [Header("----- Gun Stats -----")]
    [SerializeField] int boopDist;
    [SerializeField] int boopForce;
    [SerializeField] GameObject boopCard;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip blastSFX;
    [SerializeField] AudioClip blastPenaltySFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip dashSFX;

    Vector3 move;
    Vector3 playerVelocity;
    bool groundedPlayer;
    bool canJump;
    bool canBeat = false;
    bool hitBeat = false;
    bool hitPenalty = false;
    int HP = 1;
    float dashCooldownTimer = 0f;

    void Start()
    {
        GameManager.instance.playerDead = false;
        spawnPlayer();
    }

    void Update()
    {
        if (GameManager.instance.beatWindow)
        {
            canBeat = true;
        }
        else
        {
            if (canBeat)
            {
                hitPenalty = false;
            }
            canBeat = false;
            hitBeat = false;
        }

        ShootInput();
        DashInput();
        MovePlayer();
    }

    void MovePlayer()
    {
        float frictionForce;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            canJump = true;
            frictionForce = friction / 2;
        }
        else
        {
            frictionForce = friction;
        }

        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        if (Input.GetButtonDown("Jump") && canJump)
        {
            if (!hitPenalty)
            {
                if (canBeat && !hitBeat)
                {
                    hitBeat = true;
                    AudioManager.instance.playOnce(jumpSFX);
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                    canJump = false;
                }
                else
                {
                    hitPenalty = true;
                    BoopPenalty();
                }
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        playerVelocity.x *= frictionForce;
        playerVelocity.z *= frictionForce;

        controller.Move((move * playerSpeed + playerVelocity) * Time.deltaTime);

        if (!groundedPlayer)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void ShootInput()
    {
        if (Input.GetButtonDown("Shoot") && GameManager.instance.menuActive == null)
        {
            if (!hitPenalty)
            {
                if (canBeat && !hitBeat)
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
    }

    void Boop()
    {
        AudioManager.instance.playOnce(blastSFX);
        canJump = false;

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
    }

    IEnumerator DoDash()
    {
       float startTime = Time.time;
       Vector3 dashDirection = move; // Assuming move is the direction you want to dash in.

       // Disable gravity by storing the current playerVelocity.y
       float originalYVelocity = playerVelocity.y;
       playerVelocity.y = 0;

       while (Time.time < startTime + dashDuration)
       {
           controller.Move(dashDirection.normalized * dashSpeed * Time.deltaTime);
           yield return null;
       }

       // Re-enable gravity
       playerVelocity.y = originalYVelocity;

       dashCooldownTimer = dashCooldown;
    }

    void DashInput()
    {
        if (Input.GetButtonDown("Dash") && dashCooldownTimer <= 0 &&
            GameManager.instance.menuActive == null && Input.GetAxis("Vertical") > 0.1f)
        {
            if (!hitPenalty)
            {
                if (canBeat && !hitBeat)
                {
                    hitBeat = true;
                    AudioManager.instance.playOnce(dashSFX);
                    StartCoroutine(DoDash());
                }
                else
                {
                    hitPenalty = true;
                    BoopPenalty();
                }
            }
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }
}