using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitvity = 100;
    public Transform playerBody;
    public float xRotation = 0f;
    public float maxUpLook = 90f;
    public float maxDownLook = -90f;

    public PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        //lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitvity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitvity * Time.deltaTime;


        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxDownLook, maxUpLook);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
