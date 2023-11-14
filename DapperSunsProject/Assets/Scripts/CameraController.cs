using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Range(100, 32000)]public int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invertY;

    float xRot;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            // get input
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

            if (invertY)
            {
                xRot = +mouseY;
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

    public void UpdateSensitivty(float newSensitivity)
    {
        sensitivity = (int)newSensitivity;
    }
}