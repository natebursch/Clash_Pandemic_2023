using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkingManager : MonoBehaviourPunCallbacks
{

    public GameObject connecting;
    public GameObject multiplayer;

    public GameObject joining_multiplayer_room_connecting;
    public GameObject findMatchButton;

    // Start is called before the first frame update
    void Start()
    {

        connecting.SetActive(true);
        multiplayer.SetActive(false);

        joining_multiplayer_room_connecting.SetActive(true);
        findMatchButton.SetActive(false);

        //connect to the netwrok
        Debug.Log("Connecting to Server");
        //settings are based in resources
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            OnConnectedToMaster();
        }
        

    }

    public override void OnConnectedToMaster()
    {
        //connected to master server?
        connecting.SetActive(false);
        multiplayer.SetActive(true);



    }

    public void MultiplayerButtonPressed()
    {
        Debug.Log("Joining Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Ready for to find match");

        joining_multiplayer_room_connecting.GetComponentInChildren<TextMeshProUGUI>().text = "Connected";
        findMatchButton.SetActive(true);


    }

    public void FindMatch()
    {
        Debug.Log("Looking for Room");
        // I should then have a pop up scroll wheel or something like in hunt

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
