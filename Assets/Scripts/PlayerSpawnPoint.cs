using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawnPoint : MonoBehaviourPunCallbacks
{
    public bool hasSpawned = false;

    public PhotonView pv;

    public void Spawned(bool spawned)
    {
        hasSpawned = spawned;
        pv.RPC("SyncVar", RpcTarget.AllBuffered, hasSpawned);
    }

    [PunRPC]
    public void SyncVar(bool spawned)
    {
        hasSpawned = spawned;
    }
}
