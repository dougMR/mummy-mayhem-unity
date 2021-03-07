using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MouseMoveCameraScript : MonoBehaviour
{
    public Text message;
    private GameObject playerGO;
    private float halfWidth;
    private float halfHeight;

    // Start is called before the first frame update
    void Start()
    {
        playerGO = transform.parent.gameObject;
        halfWidth = (float)(Screen.width * 0.5);
        halfHeight = (float)(Screen.height * 0.5);
        // Start out facing forward
        transform.eulerAngles = new Vector3(playerGO.transform.eulerAngles.x, playerGO.transform.eulerAngles.y, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {


        // Mouse Movement
        // yaw += speedH * Input.GetAxis("Mouse X");
        // pitch -= speedV * Input.GetAxis("Mouse Y");

        // Find Mouse Position on screen as -1 to +1 
        if (Input.GetAxis("Mouse Y") != 0)
        { 
            // float mouseX = (Input.mousePosition.x - halfWidth) / halfWidth;
            float mouseY = -1 * (Input.mousePosition.y - halfHeight) / halfHeight;
            // mouseX = Mathf.Max(-1, Mathf.Min(1, mouseX));
            mouseY = Mathf.Max(-1, Mathf.Min(1, mouseY));

            // Chhange this to set only X-axis rotation (faster?)
            transform.eulerAngles = new Vector3(mouseY * 45 + playerGO.transform.eulerAngles.x, playerGO.transform.eulerAngles.y, 0.0f);
        }
    }

    void callResizeScreen()
    {
        halfWidth = (float)(Screen.width * 0.5);
        halfHeight = (float)(Screen.height * 0.5);
        message.text = "callResizeScreen() " + "\nhalfWidth: " + halfWidth + " / halfHeight: " + halfHeight;
    }
}
