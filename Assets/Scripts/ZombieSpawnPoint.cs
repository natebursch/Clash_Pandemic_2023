using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieSpawnPoint : MonoBehaviourPunCallbacks
{
    public bool hasSpawned;
    public int zombiesToSpawn = 1;
    public float spawnRange = 3;
    public float spawnDetectionRange = 25;

    public PhotonView photonView;

    public string[] zombieTypes;
    public string zombieType;


    public void Start()
    {
        GetComponent<SphereCollider>().radius = spawnDetectionRange;
    }


    public void OnTriggerEnter(Collider other)
    {
        //if (!PhotonNetwork.InRoom || photonView.IsMine && PhotonNetwork.IsMasterClient)

        //if the spawner has already spawned the zombie, don't spawn again.
        if (!hasSpawned && other.gameObject.tag == "Player")
        {
            Debug.Log("Spawn Enemy cuz of" + other.gameObject);

            for (int i = 0; i < zombiesToSpawn; i++)
            {
                //get a random location near the spawn point
                Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));
                GameObject newEnemy;

                //get random zombie
                zombieType = zombieTypes[Random.Range(0, zombieTypes.Length)];


                if (PhotonNetwork.InRoom)
                {
                    photonView.RPC("RPC_CreateBasicZombie_OnMaster", RpcTarget.MasterClient, spawnPos);
                }
                else
                {
                    newEnemy = Instantiate(Resources.Load(zombieType), spawnPos, Quaternion.identity) as GameObject;

                }

                hasSpawned = true;
                if (PhotonNetwork.InRoom)
                {
                    photonView.RPC("SyncVar", RpcTarget.AllBuffered, hasSpawned);
                }


            }

        }

    }
    [PunRPC]
    void RPC_CreateBasicZombie_OnMaster(Vector3 spawnPos)
    {
        PhotonNetwork.InstantiateRoomObject(zombieType, spawnPos, Quaternion.identity);
    }

    [PunRPC]
    public void SyncVar(bool spawned)
    {
        hasSpawned = spawned;
    }

}
