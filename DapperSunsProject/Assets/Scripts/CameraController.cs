using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invertY;

    float xRot;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
    }

    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            // get input
            // Vector2 input = new Vector2 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).normalized;
            
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * GameManager.instance.playerScript.sensitivity;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * GameManager.instance.playerScript.sensitivity;

            if (invertY)
            {
                xRot += mouseY;
            }
            else
            {
                xRot -= mouseY;
            }

            // clamp the rot on the X-axis
            xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);

            // rotate the camera on the X-axis
            transform.localRotation = Quaternion.Euler(xRot, 0, 0);

            // rotate the player on the Y-axis
            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
    void Restart()
    {
        xRot = Camera.main.transform.localRotation.y;
    }
}