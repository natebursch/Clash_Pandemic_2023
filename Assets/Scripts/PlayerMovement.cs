using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float speed;
    public bool isSprinting;
    public float x;
    public float z;


    // feels really fast but we can change later
    Vector3 velocity;
    public float gravity = -9.81f;

     // grounded stuff
    public bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundLayerMask;

    // jump stuff
    public float jumpHeight = 1f;

    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }
        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundLayerMask);

        if (isGrounded && velocity.y < 0)
        {
            //makes landing a little smoother, and remove the ability to float
            velocity.y = -2f;
        }
        // only get new inputs if grounded
        if (isGrounded)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        //physics idk
        //create the 3d vector to tell where the player is moving
        Vector3 move = transform.right * x + transform.forward * z;

        //walk without physics and stuff lol rip
        // time deltatime makes fram rate independent, same speed no matter what device you use
        controller.Move(move * speed * Time.deltaTime);
        //fall down
        // gravity put in manually
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);



        //jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpHeight;
            //velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            isSprinting = true;
            speed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
            speed = walkSpeed;
        }



    }
}
