using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    private Camera playerCamera;
    public CharacterController controller;
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float speed;
    public bool isSprinting;
    public bool isMoving;
    public float x;
    public float z;
    public bool isAiming;


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


    [Header("HeadBob")]
    public bool canHeadBob = true;
    public float walkBobSpeed = 14f;
    public float sprintBobSpeed = 18f;

    public float coruchBobSpeed = 8f;
    public float crouchBobAmount = .025f;

    public float walkBobAmountx = .001f;
    public float walkBobAmounty = .001f;
    public float walkBobAmountz = .001f;

    public float sprintBobAmountx = .005f;
    public float sprintBobAmounty = .005f;
    public float sprintBobAmountz = .005f;

    private float defaultYPos = 0;
    private float defaultXPos = 0;
    private float defaultZPos = 0;
    private float timerBob;


    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = controller.GetComponent<PlayerManager>().playerCamera;

        defaultXPos = playerCamera.transform.localPosition.x;
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultZPos = playerCamera.transform.localPosition.z;


    }

    // Update is called once per frame
    void Update()
    {
        
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            
            return;
        }
        //everything goes after this

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
        if (x != 0f || z!=0f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
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

        if (canHeadBob)
        {
            HandleHeadBob();
        }


    }

    public void HandleHeadBob()
    {
        if (isGrounded && isMoving && !controller.gameObject.GetComponentInChildren<WeaponManager>().isAiming)
        {
            timerBob += Time.deltaTime * (isSprinting ? sprintBobSpeed : walkBobSpeed);

            playerCamera.transform.localPosition = new Vector3(
                defaultXPos + Mathf.Sin(timerBob) * (isSprinting ? sprintBobAmountx : walkBobAmountx),
                defaultYPos + Mathf.Sin(timerBob) * (isSprinting ? sprintBobAmounty : walkBobAmounty),
                playerCamera.transform.localPosition.z
                );
        }
        else
        {
            playerCamera.transform.localPosition = new Vector3(
                defaultXPos,
                defaultYPos,
                defaultZPos
                );
        }
    }
}
