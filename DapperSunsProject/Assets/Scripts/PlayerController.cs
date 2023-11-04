using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Beat, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 20)][SerializeField] float playerSpeed;
    [Range(1, 50)][SerializeField] float jumpHeight;
    [Range(1, 5)][SerializeField] int jumpMax;
    [Range(-10, 50)][SerializeField] float gravityValue;
    [Range(0, 1)][SerializeField] float friction;

    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] int boopDist;
    [SerializeField] int boopForce;
    [SerializeField] float shootRate;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip blastSFX;
    [SerializeField] AudioClip blastPenaltySFX;
    [SerializeField] AudioClip shootSFX;

    Vector3 move;
    Vector3 playerVelocity;
    bool groundedPlayer;
    bool isShooting = false;
    bool canBoop = false;
    bool hitBeat = false;
    bool hitPenalty = false;
    int timesjumped;
    int HPOriginal;

    protected override void Start()
    {
        base.Start();

        HPOriginal = HP;
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

        if (Input.GetButtonDown("Shoot2") && !isShooting && GameManager.instance.menuActive == null)
        {
            if (!hitPenalty)
            {
                if (canBoop && !hitBeat) // && !hitBeat
                {
                    hitBeat = true;
                    Blast();
                }
                else
                {
                    hitPenalty = true;
                    BlastPenalty();
                }
            }
        }

        if (Input.GetButton("Shoot") && !isShooting && GameManager.instance.menuActive == null)
        {
            AudioManager.instance.playOnce(shootSFX);
            StartCoroutine(Shoot());
        }

        MovePlayer();
    }

    void MovePlayer()
    {
        float frictionForce;

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesjumped = 0;
            frictionForce = friction;
        }
        else
        {
            frictionForce = friction / 2;
        }

        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        if (Input.GetButtonDown("Jump") && timesjumped < jumpMax)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            timesjumped++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        playerVelocity.x *= friction;
        playerVelocity.z *= friction;

        controller.Move((move * Time.deltaTime * playerSpeed) + (playerVelocity * Time.deltaTime));
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();

            if (hit.transform != transform && damageable != null)
            {
                damageable.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void Blast()
    {
        AudioManager.instance.playOnce(blastSFX);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, boopDist))
        {
            Vector3 launchDirection = hit.normal;
            playerVelocity.x += launchDirection.x * boopForce * 2;
            playerVelocity.y += launchDirection.y * boopForce;
            playerVelocity.z += launchDirection.z * boopForce * 2;
        }
    }

    void BlastPenalty()
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
        transform.position = GameManager.instance.GetPlayerSpawnPosition();
        controller.enabled = true;

        HP = HPOriginal;
        updatePlayerUI();
    }

    public void updatePlayerUI()
    {

    }

    protected override void DoBeat()
    {
        
    }
}
