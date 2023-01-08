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

    public TextMeshProUGUI roundText;

    public bool bossRoomDiscovered;
    public bool bossRoomComplete;

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
        roundText.gameObject.SetActive(false);

    }

    public void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {

            if (enemiesAlive == 0 && !bossRoomComplete)
            {
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

            if (round == 10)
            {
                bossRoomComplete = true;

                //we want to start a timer for the people to extract
            }



        }
    }

    private void DisplayNextRound(int round)
    {
        roundText.text = "Round " + round;
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

            enemy.GetComponent<EnemyManager>().gameManager = gameObject.GetComponent<GameManager>();

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


