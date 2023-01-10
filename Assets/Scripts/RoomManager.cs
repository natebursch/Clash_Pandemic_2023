using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{

    //soingleton

    public static RoomManager Instance;
    
    private void Awake()
    {
        // delete if a new instance is created
        if (Instance || SceneManager.GetActiveScene().name =="MainMenu")
        {
            Destroy(Instance);
            return;
        }
        Debug.Log(SceneManager.GetActiveScene().name);
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
        SceneManager.sceneLoaded-=OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        //default spawsn pos
        Vector3 spawnPos = new Vector3(Random.Range(-3, 3), 2, Random.Range(-3, 3));

        //check to make sure online
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate("First_Person_Player", spawnPos, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("First_Person_Player"),spawnPos, Quaternion.identity);
        }

    }
}
