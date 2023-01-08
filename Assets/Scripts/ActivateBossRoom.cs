using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActivateBossRoom : MonoBehaviourPunCallbacks
{
    BossRoomManager bossRoomManager;

    private void Start()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            bossRoomManager = GameObject.FindObjectOfType<BossRoomManager>();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (other.gameObject.tag == "Player")
            {
                bossRoomManager.bossRoomDiscovered = true;
                bossRoomManager.roundText.gameObject.SetActive(true);
            }
        }
    }

}
