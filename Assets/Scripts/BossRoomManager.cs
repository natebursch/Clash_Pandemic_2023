using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BossRoomManager : MonoBehaviourPunCallbacks
{
    //soingleton

    public static BossRoomManager Instance;
    
    public int enemiesAlive = 0;
    public int round = 0;

    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    public List<GameObject> players;
    public GameObject[] allPlayers;

    public bool bossRoomDiscovered;
    public bool bossRoomComplete;

    //PANDEMIC HAS BEEN STARTED? IDK
    public string missionStartAnnouncement = "OUTBREAK STARTED";
    public string missionStart_tooltip = "Uh Zombies Spawn everywhere so watch the fuck out";
    public string missionEndAnnoucement = "PANDEMIC HAS BEEN CONTROLLED";
    public string missionEnd_tooltip = "Stop the team from extracting, otherwise you cannot... maybe idk sounds kinda fun.";
    public float missionAnnoucement_timer = 5f;


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

    private void Start()
    {

        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
        allPlayers = GameObject.FindGameObjectsWithTag("Player");

    }
    public void ShowRoundText(bool state)
    {
        foreach(GameObject player in players)
        {
            Debug.Log(player);
            player.GetComponentInChildren<GameManager>().roundText.gameObject.SetActive(state);
        }
    }
    public void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {

            if (enemiesAlive == 0 && !bossRoomComplete && bossRoomDiscovered)
            {
                if (round == 0)
                {
                    Debug.Log("Show Mission Start");
                    ShowMissionStatus(missionStartAnnouncement, missionStart_tooltip);
                }
                if (round == 10)
                {
                    bossRoomComplete = true;
                    ShowRoundText(false);

                    //mission complete
                    ShowMissionStatus(missionEndAnnoucement, missionEnd_tooltip);
                    //we want to start a timer for the people to extract
                    return;
                }

                round++;
                NextWave(round);

                if (PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("CurrentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRound(round);
                }

            }






        }
    }

    private void DisplayNextRound(int round)
    {
        //Debug.Log("Round" + round);
        foreach (GameObject player in players)
        {
            player.GetComponentInChildren<GameManager>().roundText.text = "Round " + round;
        }
        
    }
    public void ShowMissionStatus(string annoucement, string tooltip)
    {
        foreach(GameObject player in allPlayers)
        {
            Debug.Log("turn player canvas on");
            player.GetComponent<PlayerCanvasManager>().ShowMissionAnnouncement(annoucement, tooltip, missionAnnoucement_timer);
        }
    }

    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy;

            if (PhotonNetwork.InRoom)
            {
                enemy = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemy = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;

            }

            enemy.GetComponent<EnemyManager>().gameManager = gameObject.GetComponent<BossRoomManager>();

            enemiesAlive++;
        }

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

        Debug.Log("Player " + targetPlayer + " changed" + changedProps);
        if (photonView.IsMine)
        {
            if (changedProps["CurrentRound"] != null)
            {
                DisplayNextRound((int)changedProps["CurrentRound"]);
            }
        }

    }
}


