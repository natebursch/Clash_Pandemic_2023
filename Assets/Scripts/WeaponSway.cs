using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponSway : MonoBehaviour
{

    Vector3 startPos;
    public float swaySensitivity = 2f;
    public float swayClamp = 20f;
    public float swaySmoothness = 20f;

    Vector3 nextPos;
    Vector3 currentVelocity = Vector3.zero;
    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * swaySensitivity/100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * swaySensitivity/100 * Time.deltaTime;

        //clamp the values
        mouseX = Mathf.Clamp(mouseX, -swayClamp, swayClamp);
        mouseY = Mathf.Clamp(mouseY, -swayClamp, swayClamp);

        nextPos = new Vector3(mouseX, mouseY, 0);

        //transistion into that pos
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, nextPos + startPos, ref currentVelocity, swaySmoothness * Time.deltaTime);



    }
}
