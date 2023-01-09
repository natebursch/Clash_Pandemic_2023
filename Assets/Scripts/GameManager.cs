using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int enemiesAlive = 0;
    public int round = 0;

    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    public TextMeshProUGUI roundText;

    public GameObject endScreen;
    public TextMeshProUGUI endRounds;

    public GameObject pauseScreen;

    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        //AudioListener.volume = 1;
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
        roundText.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {

        //if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        //{
            
   


            

        //    if (enemiesAlive == 0)
        //    {
        //        round++;
        //        NextWave(round);

        //        if (PhotonNetwork.InRoom)
        //        {
        //            Hashtable hash = new Hashtable();
        //            hash.Add("CurrentRound", round);
        //            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //        }
        //        else
        //        {
        //            DisplayNextRound(round);
        //        }

        //    }

        //    if (round == 10)
        //    {
                

        //        //we want to start a timer for the people to extract
        //    }



        



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

            enemy.GetComponent<EnemyManager>().gameManager = gameObject.GetComponent<BossRoomManager>();

            enemiesAlive++;
        }

    }
    public void EndGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        endScreen.SetActive(true);
        //AudioListener.volume = 0;
        //make player notmovable
        //should probably also delete the player body idk we can just freeze it
        gameObject.GetComponent<PlayerCanvasManager>().DisableCharacter(true);


        Cursor.lockState = CursorLockMode.None;

        endRounds.text = "Rounds Survived: " + round;

    }

    public void ReplayButtonPressed()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Game");
    }

    public void MainMenuButtonPressed()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        else
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
        
    }


    public void Pause()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        Cursor.lockState = CursorLockMode.None;
       //AudioListener.volume = 0;
       pauseScreen.SetActive(true);
    }

    public void UnPause()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        Cursor.lockState = CursorLockMode.Locked;
        //AudioListener.volume = 1;
        pauseScreen.SetActive(false);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

        Debug.Log("Player " + targetPlayer + " changed" + changedProps);
        if (photonView.IsMine)
        {
            if (changedProps["CurrentRound"]!= null)
            {
                DisplayNextRound((int)changedProps["CurrentRound"]);
            }
        }

    }
}
