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
    //things needed for singleplayer
    public PlayerCanvasManager singlePlayer;

    public bool bossRoomDiscovered;
    public int roundsToComplete = 2;
    public bool bossRoomComplete;

    //PANDEMIC HAS BEEN STARTED? IDK
    public string missionStartAnnouncement = "OUTBREAK STARTED";
    public string missionStart_tooltip = "Uh Zombies Spawn everywhere so watch the fuck out";
    public string missionEndAnnoucement = "PANDEMIC HAS BEEN CONTROLLED";
    public string missionEnd_tooltip = "Stop the team from extracting, otherwise you cannot... maybe idk sounds kinda fun.";
    public float missionAnnoucement_timer = 5f;

    // stuff that happens outside the bossroom.
    public int zombiesToSpawnPerSpawner = 15;
    public GameObject[] spawners;

    //reward
    public GameObject Bounty_Reward;
    public GameObject spawnedBounty;
    public Transform BountyReward_SpawnLocation;
    public string bountyHasBeenGrabbed_Announcement = "BOUNTY HAS BEEN PICKED UP";
    public string bountyHasBeenGrabbed_tooltip = "Kill the player with the Cowboy Hat";


    public string[] zombieTypes;
    private string zombieType;


    private void Awake()
    {
        // delete if a new instance is created
        if (PhotonNetwork.InRoom)
        {
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


    }

    private void Start()
    {

        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");


        // not getting the other connected players :OOOOOO
        //only gets first connected player
        //make an RPC that is called to change this value...
        //allPlayers = GameObject.FindGameObjectsWithTag("Player");

    }
    public void ShowRoundText(bool state)
    {
        foreach(GameObject player in players)
        {
            if (player != null)
            {
                if (PhotonNetwork.InRoom)
                {

                    player.GetComponentInChildren<PlayerCanvasManager>().photonView.RPC("ShowRound_Text", RpcTarget.All, state);

                }
                else
                {

                    player.GetComponentInChildren<PlayerCanvasManager>().ShowRound_Text(true);

                }
            }
            
        }  
    }
    public void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
        {

            if (enemiesAlive == 0 && !bossRoomComplete && bossRoomDiscovered)
            {

                if (round == 0)
                {
                    allPlayers = GameObject.FindGameObjectsWithTag("Player");
                    Debug.Log("Show Mission Start");
                    ShowMissionStatus(missionStartAnnouncement, missionStart_tooltip);

                    // spawn a ton of zombies all over the place
                    // amount per spawn point
                    // list of places to spawn
                    // an rpc that tells the master client to spawn all of the zombies
                    StartOutside_Events();

                }
                if (round == roundsToComplete)
                {
                    bossRoomComplete = true;
                    ShowRoundText(false);

                    //mission complete
                    ShowMissionStatus(missionEndAnnoucement, missionEnd_tooltip);
                    //we want to start a timer for the people to extract
                    //we want to instantiate something that the players inside have to pick up
                    SpawnBounty();

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

        if (PhotonNetwork.InRoom || players.Count > 0)
        {
            foreach (GameObject player in players)
            {
                if (player != null)
                {
                    if (PhotonNetwork.InRoom)
                    {

                        player.GetComponentInChildren<PlayerCanvasManager>().photonView.RPC("ShowMissionRound_Text", RpcTarget.All, round);

                    }
                    else
                    {
                        player.GetComponentInChildren<PlayerCanvasManager>().ShowMissionRound_Text(round);
                    }
                }

            }
        }
        else
        {
            singlePlayer.roundText.SetActive(true);
            singlePlayer.roundText.GetComponent<TextMeshProUGUI>().text = "Round: " + round;

        }


    }
    public void ShowMissionStatus(string annoucement, string tooltip)
    {
        if (allPlayers[0]!=null)
        {
            foreach (GameObject player in allPlayers)
            {
                Debug.Log("turn player canvas on");
                if (player != null)
                {
                    if (PhotonNetwork.InRoom)
                    {

                        player.GetComponent<PlayerCanvasManager>().photonView.RPC("ShowMissionAnnouncementRPC", RpcTarget.All, annoucement, tooltip, missionAnnoucement_timer);

                    }
                    else
                    {
                        player.GetComponent<PlayerCanvasManager>().ShowMissionAnnouncementRPC(annoucement,tooltip, missionAnnoucement_timer);
                    }
                }
            }
        }

    }

    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy;

            //get random zombie
            zombieType = zombieTypes[Random.Range(0, zombieTypes.Length)];


            if (PhotonNetwork.InRoom)
            {
                enemy = PhotonNetwork.Instantiate(zombieType, spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemy = Instantiate(Resources.Load(zombieType), spawnPoint.transform.position, Quaternion.identity) as GameObject;
                enemy.GetComponent<ZombieBasicManager>().attackDistance = 1000;

            }

            if (enemy.GetComponent<EnemyManager>() != null)
            {
                enemy.GetComponent<EnemyManager>().gameManager = gameObject.GetComponent<BossRoomManager>();

            }
            else
            {
                enemy.GetComponent<ZombieBasicManager>().gameManager = gameObject.GetComponent<BossRoomManager>();
                enemy.GetComponent<ZombieBasicManager>().inBossRoom = true;

            }


            enemiesAlive++;
        }

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

        Debug.Log("Player " + targetPlayer + " changed" + changedProps);
        if (PhotonNetwork.InRoom)
        {
            if (changedProps["CurrentRound"] != null)
            {
                DisplayNextRound((int)changedProps["CurrentRound"]);
            }
        }

    }

    #region outside factors

    public void StartOutside_Events()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_StartOutside_Events", RpcTarget.MasterClient, zombiesToSpawnPerSpawner);
        }

    }

    [PunRPC]
    public void RPC_StartOutside_Events(int amount)
    {
        foreach (GameObject spawner in spawners)
        {
            spawner.GetComponent<BossRoom_OutsideEvent_Spawner>().Activate_OutsideSpawn_Event(amount);
        }
    }
    #endregion

    #region BossRoom_Bounty
    public void SpawnBounty()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_SpawnBounty", RpcTarget.MasterClient, zombiesToSpawnPerSpawner);
        }
        else
        {
            RPC_SpawnBounty(1);
        }

    }

    public void DestroyBounty()
    {
        //NOT USED
        Destroy(spawnedBounty);
    }

    [PunRPC]
    public void RPC_SpawnBounty(int amount)
    {
        if (PhotonNetwork.InRoom)
        {

            spawnedBounty = PhotonNetwork.InstantiateRoomObject("Boss Reward", BountyReward_SpawnLocation.position, BountyReward_SpawnLocation.rotation);

        }
        else
        {
            spawnedBounty = Instantiate(Resources.Load("Boss Reward"), BountyReward_SpawnLocation.position, BountyReward_SpawnLocation.rotation) as GameObject;

        }
    }
    #endregion
}


