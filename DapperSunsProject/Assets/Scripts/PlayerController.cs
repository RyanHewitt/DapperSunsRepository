using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

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

    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int timesjumped;
    bool isShooting;
    int HPOriginal;


    private void Start()
    {
        HPOriginal = HP;
    }

    void Update()
    {
        if (Input.GetButton("Shoot") && !isShooting)
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

    public void takeDamage(int amount)
    {
        HP -= amount;
        //updatePlayerUI();
        //StartCoroutine(GameManager.instance.playerFlashDamage());
        //if (HP <= 0)
        //{
        //    GameManager.instance.youLose();
        //}
    }

    //public void spawnPlayer()
    //{
    //    controller.enabled = false;
    //    HP = HPOriginal;
    //    transform.position = GameManager.instance.playerSpawnPos.transform.position;
    //    controller.enabled = true;
    //}

    //public void updatePlayerUI()
    //{
    //    GameManager.instance.PlayerHPBar.fillAmount = (float)HP / HPOriginal;
    //}
}
