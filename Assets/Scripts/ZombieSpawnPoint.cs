using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieSpawnPoint : MonoBehaviour
{
    public bool hasSpawned;
    public int zombiesToSpawn = 1;
    public float spawnRange = 3;
    public float spawnDetectionRange = 25;


    public void Start()
    {
        GetComponent<SphereCollider>().radius = spawnDetectionRange;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            //if not a master client
            return;
        }

        //if the spawner has already spawned the zombie, don't spawn again.
        if (!hasSpawned && other.gameObject.tag == "Player")
        {
            Debug.Log("Spawn Enemy cuz of" + other.gameObject);

            for (int i = 0; i < zombiesToSpawn; i++)
            {
                //get a random location near the spawn point
                Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));
                GameObject newEnemy;

                if (PhotonNetwork.InRoom)
                {
                    newEnemy = PhotonNetwork.Instantiate("ZombieBasic", spawnPos, Quaternion.identity);
                }
                else
                {
                    newEnemy = Instantiate(Resources.Load("ZombieBasic"), spawnPos, Quaternion.identity) as GameObject;

                }

                hasSpawned = true;

            }

        }
        

        //idk where i wanna go with this yet

        // basically was thinking only to spawn zombie when a player is close enough... but idk anymroe....





    }

}
