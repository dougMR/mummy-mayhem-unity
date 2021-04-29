using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraByMouseSpeedScript : MonoBehaviour
{
    public float mouseSensitivity = 32f;
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -65f, 75f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
