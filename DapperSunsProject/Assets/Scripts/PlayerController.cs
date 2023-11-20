using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Range(0, 1)][SerializeField] float frictionAir;
    [Range(0, 1)][SerializeField] float frictionGround;

    [Header("----- Dash Stats -----")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] int dashCooldown;

    [Header("----- Slam Stats -----")]
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
    [SerializeField] AudioClip slamStartSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip damageSFX;

    [Range(100, 2500)] public float sensitivity;

    GameObject ghost;
    Transform startPos;

    Vector3 translation;
    Vector3 move;
    Vector3 playerVelocity;
    Vector3 boopVelocity;
    Vector3 boopVelocityOg;
    Vector3 jumpVelocity;
    Vector3 jumpVelocityOg;
    bool groundedPlayer;
    bool canBeat = false;
    bool hitBeat = false;
    bool hitPenalty = false;
    bool slamming = false;
    bool dashing = false;
    int HP = 1;
    int dashCounter;
    float originalGravity;
    float frictionForce;
    float boopElapsedTime;
    float jumpElapsedTime;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
        GameManager.instance.OnBeatEvent += DoBeat;

        startPos = GameManager.instance.GetPlayerSpawn().transform;
        ghost = new GameObject("ghost");
        ghost.transform.position = startPos.position;

        GameManager.instance.playerDead = false;
        SpawnPlayer();

        originalGravity = gravityValue;
    }

    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

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
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isPaused)
        {
            ShootInput();
            MovePlayer();
            DashInput();
            SlamInput();

            ghost.transform.position = transform.position; 
        }
    }

    public void Ground(Transform ground)
    {
        ghost.transform.parent = ground;
        ghost.transform.position = transform.position;
        groundedPlayer = true;
        frictionForce = frictionGround;
    }

    public void Unground()
    {
        groundedPlayer = false;
        ghost.transform.parent = null;
        frictionForce = frictionAir;
    }

    void Restart()
    {
        Destroy(ghost);
        ghost = new GameObject("ghost");
        SpawnPlayer();

        playerVelocity = Vector3.zero;
        jumpVelocity = Vector3.zero;
        boopVelocity = Vector3.zero;
        jumpElapsedTime = 1;
        boopElapsedTime = 1;

        slamming = false;
        gravityValue = originalGravity;
    }

    void MovePlayer()
    {
        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            if (!hitPenalty)
            {
                if (canBeat && !hitBeat)
                {
                    hitBeat = true;
                    AudioManager.instance.audioSource.PlayOneShot(jumpSFX);

                    jumpVelocity.y += jumpHeight;
                    jumpVelocityOg = jumpVelocity;
                    jumpElapsedTime = 0f;
                }
                else
                {
                    hitPenalty = true;
                    BoopPenalty();
                }
            }
        }

        boopElapsedTime += Time.deltaTime;
        boopVelocity = Vector3.Lerp(boopVelocityOg, Vector3.zero, boopElapsedTime);
        Vector3 boopVector = new Vector3(boopVelocity.x, 0, boopVelocity.z);
        boopVector *= frictionForce;

        if (!dashing && !slamming)
        {
            playerVelocity.x *= frictionForce;
            playerVelocity.z *= frictionForce;

            translation = (ghost.transform.position - transform.position);

            controller.Move(((move * playerSpeed + playerVelocity + boopVector) * Time.deltaTime) + translation);
        }

        CheckHeadHit();

        if (!groundedPlayer)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
        else
        {
            playerVelocity.y = 0f;
        }

        boopVector = new Vector3(0, boopVelocity.y, 0);

        jumpElapsedTime += Time.deltaTime;
        jumpVelocity = Vector3.Lerp(jumpVelocityOg, Vector3.zero, jumpElapsedTime);

        controller.Move((playerVelocity + boopVector + jumpVelocity) * Time.deltaTime);
    }

    void CheckHeadHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(headPos.transform.position, Vector3.up, out hit, 0.4f))
        {
            if (hit.transform.position != transform.position)
            {
                playerVelocity.y = -playerVelocity.y;
                boopVelocity.y = 0f;
                jumpVelocity.y = 0f;
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
            AudioManager.instance.audioSource.PlayOneShot(damageSFX);
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

        ghost.transform.position = startPos.position;
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;

        controller.enabled = true;

        HP = 1;
    }

    void DashInput()
    {
        if (Input.GetButtonDown("Dash") && move.normalized.magnitude > 0)
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
        dashing = true;

        Vector3 dashDirection = move.normalized;
        playerVelocity.y = 0;
        dashCounter = dashCooldown;
        playerVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        dashing = false;
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
                    AudioManager.instance.audioSource.PlayOneShot(slamStartSFX);
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
        if (slamming)
        {
            AudioManager.instance.audioSource.PlayOneShot(slamSFX);
            SlamImpact(); 
        }

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
                    DoBoop(Vector3.up / 2);
                }
            }
        }
    }

    void DoBeat()
    {
        
    }

    public void DoBoop(Vector3 direction)
    {
        playerVelocity.y = 0;
        boopVelocity = direction * boopForce;
        boopVelocityOg = boopVelocity;
        boopElapsedTime = 0f;
    }
}