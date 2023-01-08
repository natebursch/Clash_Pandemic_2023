using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieSpawnPoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            //if not a master client
            return;
        }


        //idk where i wanna go with this yet

        // basically was thinking only to spawn zombie when a player is close enough... but idk anymroe....
    }

}
