using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        playerSpawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawner");

        Vector3 spawnPos = FindSpawnPoint().transform.position;


        //check to make sure online
        if (PhotonNetwork.InRoom)
        {


            GameObject player = PhotonNetwork.Instantiate("First_Person_Player", spawnPos, Quaternion.identity);

            if (PhotonNetwork.CurrentRoom.CustomProperties["CurrentTeamNumber"] != null)
            {
                teamNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentTeamNumber"];
            }
            else
            {
                teamNumber = 0;
            }
            Debug.Log(teamNumber);

            player.gameObject.GetComponent<PlayerManager>().teamNumber = teamNumber;

            Hashtable hash = new Hashtable();
            teamNumber++;
            hash.Add("CurrentTeamNumber", teamNumber);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);


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
        playerSpawnPoint.GetComponent<PlayerSpawnPoint>().Spawned(true);


        return playerSpawnPoint;
    }

    public GameObject validPoint()
    {
        int spawnPoint = Random.Range(0, playerSpawnPoints.Length);
        if (playerSpawnPoints[spawnPoint].gameObject.GetComponent<PlayerSpawnPoint>().hasSpawned)
        {
            Debug.Log(playerSpawnPoints[spawnPoint].gameObject.GetComponent<PlayerSpawnPoint>().hasSpawned);
            GameObject spawn = playerSpawnPoints[spawnPoint] = validPoint();
        }

        return playerSpawnPoints[spawnPoint];
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

        if (propertiesThatChanged["CurrentTeamNumber"] != null)
        {
            teamNumber = (int)propertiesThatChanged["CurrentTeamNumber"];
            Debug.Log("teamNumber" + teamNumber);
        }
    }
    


}
