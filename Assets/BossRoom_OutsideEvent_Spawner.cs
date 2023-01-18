using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossRoom_OutsideEvent_Spawner : MonoBehaviour
{

    public string[] zombieTypes;
    public string zombieType;

    public void Activate_OutsideSpawn_Event(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //get random zombie
            zombieType = zombieTypes[Random.Range(0, zombieTypes.Length)];

            //Debug.Log("Penis");
            PhotonNetwork.InstantiateRoomObject(zombieType, transform.position, Quaternion.identity);
        }
    }
}
