using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("----- Components-----")]
    [SerializeField] CharacterController controller;
    [Range(1, 20)][SerializeField] float playerSpeed;
    [Range(1, 50)][SerializeField] float jumpHeight;
    [Range(-10, 50)][SerializeField] float gravityValue;

    [Header("-----Player Stats-----")]
    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private void Start()
    {

    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);


        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += jumpHeight;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
