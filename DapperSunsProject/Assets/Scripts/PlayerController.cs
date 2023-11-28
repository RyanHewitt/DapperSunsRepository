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
                if (!canBeat) // First update in window
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
                if (canBeat) // First update off window
                {
                    hitPenalty = false;
                    hitBeat = false;
                }

                canBeat = false;
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
            if (!hitBeat && !hitPenalty)
            {
                hitBeat = true;
                AudioManager.instance.audioSource.PlayOneShot(jumpSFX);

                jumpVelocity.y += jumpHeight;
                jumpVelocityOg = jumpVelocity;
                jumpElapsedTime = 0f;

                if (boopVelocity.y < 0f)
                {
                    boopVelocityOg.y = 0f;
                }

                if (!canBeat)
                {
                    BoopPenalty();
                }
            }
            else
            {
                BoopPenalty();
            }
        }

        boopElapsedTime += Time.deltaTime;
        boopVelocity = Vector3.Lerp(boopVelocityOg, Vector3.zero, boopElapsedTime);
        Vector3 boopVector = new Vector3(boopVelocity.x, 0, boopVelocity.z);
        boopVector *= frictionForce;

        // Cancel boop velocity if you move in the opposite direction or stop moving
        if (move.normalized.magnitude > 0)
        {
            if (move.x == 0f || Mathf.Sign(move.x) != Mathf.Sign(boopVector.x))
            {
                boopVelocityOg.x = 0f;
            }

            if (move.z == 0f || Mathf.Sign(move.z) != Mathf.Sign(boopVector.z))
            {
                boopVelocityOg.z = 0f;
            }
        }

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
        Vector3 verticalVelocity = (playerVelocity + boopVector + jumpVelocity) * Time.deltaTime;

        controller.Move(verticalVelocity);
    }

    void CheckHeadHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(headPos.transform.position, Vector3.up, out hit, 0.4f))
        {
            if (hit.transform.position != transform.position && !hit.collider.isTrigger)
            {
                playerVelocity.y = -playerVelocity.y;
            }
        }
    }

    void ShootInput()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            if (!hitBeat && !hitPenalty)
            {
                hitBeat = true;
                Boop();

                if (!canBeat)
                {
                    BoopPenalty();
                }
            }
            else
            {
                BoopPenalty();
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
        hitPenalty = true;
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
            if (!hitBeat && !hitPenalty && dashCounter <= 0)
            {
                hitBeat = true;
                AudioManager.instance.playOnce(dashSFX);
                StartCoroutine(GameManager.instance.FlashLines(dashDuration));
                StartCoroutine(DoDash());

                if (!canBeat)
                {
                    BoopPenalty();
                }
            }
            else
            {
                BoopPenalty();
            }
        }
    }

    IEnumerator DoDash()
    {
        dashing = true;

        Vector3 dashDirection = move.normalized;

        playerVelocity.y = 0;
        boopVelocityOg = Vector3.zero;
        jumpVelocityOg = Vector3.zero;

        dashCounter = dashCooldown;
        playerVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        dashing = false;
    }

    void SlamInput()
    {
        if (Input.GetButtonDown("Slam") && !groundedPlayer && !slamming)
        {
            if (!hitBeat && !hitPenalty)
            {
                hitBeat = true;
                AudioManager.instance.audioSource.PlayOneShot(slamStartSFX);
                StartCoroutine(DoSlam());

                if (!canBeat)
                {
                    BoopPenalty();
                }
            }
            else
            {
                BoopPenalty();
            }
        }
    }

    IEnumerator DoSlam()
    {
        slamming = true;

        // Disable player horizontal control and gravity influence here if desired
        gravityValue = 0;
        boopVelocityOg = Vector3.zero;
        jumpVelocityOg = Vector3.zero;

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
        if (Physics.BoxCast(transform.position - Vector3.down, Vector3.one, Vector3.down, out hit))
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