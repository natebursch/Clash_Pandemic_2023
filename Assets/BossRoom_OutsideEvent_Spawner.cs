using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossRoom_OutsideEvent_Spawner : MonoBehaviour
{
    public void Activate_OutsideSpawn_Event(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //Debug.Log("Penis");
            PhotonNetwork.InstantiateRoomObject("ZombieBasic", transform.position, Quaternion.identity);
        }
    }
}
