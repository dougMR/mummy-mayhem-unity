/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
*/

/*
This Script is Deactivated.  
Player Movement is handled in PlayerMovementScript.cs
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerScript : MonoBehaviour
{
    // Start is called before the first frame update


    float inputX, inputZ, moveSpeed = 5, rotateSpeed = 50;
    // public Text messageText;
    private Rigidbody rb;
    private CharacterController controller;
    private float jumpForce = 8;
    RaycastHit hit;

    void Start()
    {
        // string xyzString = "0,0,90";
        // messageText.text = "xyzString: " + xyzString;
        // callRotateXYZ(xyzString);
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        if (inputX != 0)
            rotate();
        if (inputZ != 0)
            move();

        // Jump   
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Debug.Log("JUMP");
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    private bool isGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            float dist = hit.distance;
            Debug.Log("dist: " + dist);
            if (dist < 2.5)
            {

                return true;
            }
            else
            {

                return false;
            }
        }
        return false;
    }


    private void rotate()
    {
        // messageText.text = "inputX: " + inputX;
        transform.Rotate(new Vector3(0f, inputX * Time.deltaTime * rotateSpeed, 0f));
    }


    private void move()
    {
        transform.position += transform.forward * inputZ * Time.deltaTime * moveSpeed;
    }

}
