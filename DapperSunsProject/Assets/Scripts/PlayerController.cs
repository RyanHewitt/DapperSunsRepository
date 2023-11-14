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
    [SerializeField] Transform headPos;

    [Header("----- Player Stats -----")]
    [Range(1, 20)][SerializeField] float playerSpeed;
    [Range(1, 50)][SerializeField] float jumpHeight;
    [Range(-10, 50)][SerializeField] float gravityValue;
    [Range(0, 1)][SerializeField] float friction;

    [Header("----- Dash Stats -----")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] int dashCooldown;

    [Header("----- Ground Pound Stats -----")]
    [SerializeField] float slamSpeed;

    [Header("----- Gun Stats -----")]
    [SerializeField] int boopDist;
    [SerializeField] int boopForce;
    [SerializeField] GameObject boopCard;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip boopSFX;
    [SerializeField] AudioClip blastPenaltySFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] AudioClip slamSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip damageSFX;

    Transform startPos;

    Vector3 move;
    Vector3 playerVelocity;
    bool groundedPlayer;
    bool canJump;
    bool canBeat = false;
    bool hitBeat = false;
    bool hitPenalty = false;
    bool slamming = false;
    int HP = 1;
    int dashCounter;
    float originalGravity;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
        GameManager.instance.OnBeatEvent += DoBeat;

        startPos = GameManager.instance.GetPlayerSpawn().transform;

        GameManager.instance.playerDead = false;
        SpawnPlayer();

        originalGravity = gravityValue;
    }

    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            if (GameManager.instance.beatWindow)
            {
                if (!canBeat) // First beat in window
                {
                    if (dashCounter > 0)
                    {
                        dashCounter--;
                    }
                }

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
            MovePlayer();
            DashInput();
            SlamInput(); 
        }
    }

    void Restart()
    {
        SpawnPlayer();
        playerVelocity = Vector3.zero;
        slamming = false;
        gravityValue = originalGravity;
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

        if (slamming)
        {
            // If we're ground pounding, we only want to apply the downward velocity
            playerVelocity.x = 0;
            playerVelocity.z = 0;
            move = Vector3.zero;
        }

        playerVelocity.x *= frictionForce;
        playerVelocity.z *= frictionForce;

        controller.Move((move * playerSpeed + playerVelocity) * Time.deltaTime);

        CheckHeadHit();

        if (!groundedPlayer)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void CheckHeadHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(headPos.transform.position, Vector3.up, out hit, 0.4f))
        {
            if (hit.transform.position != transform.position)
            {
                playerVelocity.y = -playerVelocity.y; 
            }
        }
    }

    void ShootInput()
    {
        if (Input.GetButtonDown("Shoot"))
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
        AudioManager.instance.audioSource.PlayOneShot(boopSFX);
        canJump = false;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, boopDist))
        {
            if (!hit.collider.isTrigger && hit.transform.position != transform.position)
            {
                DoBoop(hit.normal);

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

        if (damageSFX != null)
        {
            AudioManager.instance.Play3D(damageSFX, transform.position);
        }


        if (HP <= 0)
        {
            AudioManager.instance.audioSource.PlayOneShot(deathSFX);
            GameManager.instance.playerDead = true;
            GameManager.instance.PopupLose();
        }
    }

    public void SpawnPlayer()
    {
        controller.enabled = false;

        transform.position = startPos.position;
        transform.rotation = startPos.rotation;

        controller.enabled = true;

        HP = 1;
    }

    void DashInput()
    {
        if (Input.GetButtonDown("Dash") && Input.GetAxis("Vertical") > 0.1f)
        {
            if (!hitPenalty)
            {
                if (canBeat && !hitBeat && dashCounter <= 0)
                {
                    hitBeat = true;
                    AudioManager.instance.playOnce(dashSFX);
                    StartCoroutine(GameManager.instance.FlashLines(dashDuration));
                    StartCoroutine(DoDash());
                }
                else
                {
                    hitPenalty = true;
                    BoopPenalty();
                }
            }
        }
    }

    IEnumerator DoDash()
    {
        float startTime = Time.time;
        Vector3 dashDirection = transform.forward;
        playerVelocity.y = 0;
        dashCounter = dashCooldown;           

        while (Time.time < startTime + dashDuration)
        {
            controller.Move(dashDirection.normalized * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void SlamInput()
    {
        if (Input.GetButtonDown("Slam") && !groundedPlayer && !slamming)
        {
            if (!hitPenalty)
            {
                if (canBeat && !hitBeat)
                {
                    hitBeat = true;
                    AudioManager.instance.playOnce(slamSFX);
                    StartCoroutine(DoSlam());
                }
                else
                {
                    hitPenalty = true;
                    BoopPenalty();
                }
            }
        }
    }

    IEnumerator DoSlam()
    {
        slamming = true;

        // Disable player horizontal control and gravity influence here if desired
        gravityValue = 0;

        // Apply the ground pound speed directly downwards
        playerVelocity.y = -slamSpeed;

        // Wait until the player hits the ground
        while (!groundedPlayer)
        {
            yield return null;
        }

        // Apply the slam impact logic (e.g., damage enemies, create an impact effect, etc.)
        SlamImpact();

        // Reset gravity influence
        gravityValue = originalGravity;

        slamming = false;
    }

    void SlamImpact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, boopDist))
        {
            if (!hit.collider.isTrigger)
            {
                IBoop boopable = hit.collider.GetComponent<IBoop>();

                if (hit.transform != transform && boopable != null)
                {
                    boopable.DoBoop(boopForce, true);
                    playerVelocity.y += boopForce;
                }
            }
        }
    }

    void DoBeat()
    {
        
    }

    public void DoBoop(Vector3 direction)
    {
        playerVelocity.x += direction.x * boopForce * 2;
        playerVelocity.y += direction.y * boopForce;
        playerVelocity.z += direction.z * boopForce * 2;
    }
}