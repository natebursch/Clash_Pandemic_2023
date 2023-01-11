using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ActivateBossRoom : MonoBehaviourPunCallbacks
{
    BossRoomManager bossRoomManager;
    bool isColliding = false;

    private void Start()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
        {
            bossRoomManager = GameObject.FindObjectOfType<BossRoomManager>();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        //have a feeling if two different people enter now that they wont be detected but it is what it is....
        //It should still work since the rpc is happening locally.
        if (isColliding) return;
        isColliding = true;
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
        {

            if (other.gameObject.tag == "Player" && !bossRoomManager.bossRoomComplete)
            {
                bossRoomManager.bossRoomDiscovered = true;
                
                if (bossRoomManager.players.Contains(other.gameObject))
                {
                    bossRoomManager.players.Remove(other.gameObject);
                }
                else
                {
                    bossRoomManager.players.Add(other.gameObject);
                }

                bossRoomManager.ShowRoundText(true);

            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        isColliding=false;
    }

}
