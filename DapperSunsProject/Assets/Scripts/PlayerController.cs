using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Beat, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerSpawnPos;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 20)][SerializeField] float playerSpeed;
    [Range(1, 50)][SerializeField] float jumpHeight;
    [Range(1, 5)][SerializeField] int jumpMax;
    [Range(-10, 50)][SerializeField] float gravityValue;

    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip blastSFX;

    Vector3 move;
    Vector3 playerVelocity;
    bool groundedPlayer;
    bool isShooting;
    bool canBlast;
    bool hitBeat;
    int timesjumped;
    int HPOriginal;

    protected override void Start()
    {
        base.Start();

        HPOriginal = HP;
        GameManager.instance.playerDead = false;
        GameManager.instance.SetPlayerSpawnPosition(playerSpawnPos.transform.position);
        spawnPlayer();
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time - timer < ((60f / bpm) / 5) || Time.time - timer > ((60f / bpm) / 1.25)) // Given 120 bpm, has window of 0.1 seconds before and after the beat
        {
            canBlast = true;
        }
        else
        {
            canBlast = false;
            hitBeat = false;
        }

        if (Input.GetButtonDown("Shoot2") && canBlast && !isShooting && GameManager.instance.menuActive == null)
        {
            if (!hitBeat)
            {
                hitBeat = true;
                Blast();
            }
        }

        if (Input.GetButton("Shoot") && !isShooting && GameManager.instance.menuActive == null)
        {
            StartCoroutine(Shoot());
        }

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesjumped = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        if (Input.GetButtonDown("Jump") && timesjumped < jumpMax)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            timesjumped++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
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
