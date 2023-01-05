using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{

    public GameObject connecting;
    public GameObject multiplayer;

    // Start is called before the first frame update
    void Start()
    {

        connecting.SetActive(true);
        multiplayer.SetActive(false);

        //connect to the netwrok
        Debug.Log("Connecting to Server");
        //settings are based in resources
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {

        Debug.Log("Joing Lobby");
        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Ready for Multiplayer");
        //disable connection
        connecting.SetActive(false);
        multiplayer.SetActive(true);

    }

    public void FindMatch()
    {
        Debug.Log("Looking for Room");

        //join a random lobby
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeRoom();

    }

    void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);

        // create options for room
        // https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_realtime_1_1_room_options.html
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            //clsoe it when max platers
            IsOpen = true,
            MaxPlayers = 6,
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom("RoomName_" + randomRoomName, roomOptions);
        Debug.Log("Room Made: " + randomRoomName);

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Loading Scene 1");
        PhotonNetwork.LoadLevel(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
