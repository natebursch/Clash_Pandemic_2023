using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActivateBossRoom : MonoBehaviourPunCallbacks
{
    BossRoomManager bossRoomManager;

    private void Start()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
        {
            bossRoomManager = GameObject.FindObjectOfType<BossRoomManager>();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
        {
            if (other.gameObject.tag == "Player" && !bossRoomManager.bossRoomComplete)
            {
                bossRoomManager.bossRoomDiscovered = true;
                bossRoomManager.ShowRoundText(true);
                if (bossRoomManager.players.Contains(other.gameObject))
                {
                    //bossRoomManager.players.Remove(other.gameObject);
                }
                else
                {
                    bossRoomManager.players.Add(other.gameObject);
                }
                
            }
        }
    }

}
