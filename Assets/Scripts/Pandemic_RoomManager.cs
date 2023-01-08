using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pandemic_RoomManager : MonoBehaviourPunCallbacks
{

    //soingleton

    public static Pandemic_RoomManager Instance;
    public GameObject[] playerSpawnPoints;
    public int teamNumber = 0;

    private void Awake()
    {
        // delete if a new instance is created
        if (Instance)
        {
            Destroy(Instance);
            return;
        }

        //dont destroy the first room instance
        // and make this instance the singleton
        Instance = this;
        DontDestroyOnLoad(Instance);

    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {


        //default spawsn pos
        playerSpawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawner");
        Vector3 spawnPos = FindSpawnPoint().transform.position;

        //check to make sure online
        if (PhotonNetwork.InRoom)
        {
            GameObject player = PhotonNetwork.Instantiate("First_Person_Player", spawnPos, Quaternion.identity);
            player.gameObject.GetComponent<PlayerManager>().teamNumber = teamNumber;
            teamNumber++;
        }
        else
        {
            Object player = Instantiate(Resources.Load("First_Person_Player"), spawnPos, Quaternion.identity);
            //Object player = Instantiate(Resources.Load("Sci_fi_Character"), spawnPos, Quaternion.identity);

        }

    }

    public GameObject FindSpawnPoint()
    {
        GameObject playerSpawnPoint = validPoint();
        playerSpawnPoint.GetComponent<PlayerSpawnPoint>().hasSpawned = true;

        return playerSpawnPoint;
    }

    public GameObject validPoint()
    {
        GameObject playerSpawnPoint;

        int spawnPoint = Random.Range(0, playerSpawnPoints.Length);

        playerSpawnPoint = playerSpawnPoints[spawnPoint];

        if (playerSpawnPoint.GetComponent<PlayerSpawnPoint>().hasSpawned == true)
            {
                playerSpawnPoint = validPoint();
            }

        return playerSpawnPoint;

    }
}
