using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extraction_Point : MonoBehaviour
{
    public bool canExtract;
    public bool playerInZone;
    public bool enemyInZone;
    public bool extractionBlocked;


    public bool enemyTimerTriggered;

    public bool timerOn;
    public float timer = 0;
    public string timerText;

    public int timeToExtract = 10;
    public int timeToExtractWithEnemy;

    //team in zone with bounty
    public List<GameObject> playersInZone;
    public List<GameObject> enemiesInZone;
    public int team = -1;

    PlayerManager pm;


    private void Update()
    {
        if (enemyInZone || !playerInZone)
        {
            timer = 0;
        }
        foreach (GameObject player in playersInZone)
        {

        }
        if (playerInZone && !enemyInZone)
        {
            timer += Time.deltaTime;
        }

        if (timer >= timeToExtract)
        {


            if (playersInZone.Count <= 0 )
            {
                timer = 0;
                playerInZone = false;
            }
            else
            {
                foreach (GameObject player in playersInZone)
                {
                    player.GetComponent<PlayerManager>().EndGame_Bounty(player.GetComponent<PlayerManager>().photonView.ViewID);
                }
            }

            //PlayerManager[] players = FindObjectsOfType<PlayerManager>();
            //foreach (PlayerManager player in players)
            //{

            //}
        }
    }
    public void PlayerEnteredExtractionZone(GameObject player)
    {
        if (player.GetComponent<PlayerManager>().hasBounty)
        {
            pm = player.GetComponent<PlayerManager>();

            team = pm.teamNumber;

            playerInZone = true;

            if (!playersInZone.Contains(player))
            {
                playersInZone.Add(player);
            }
            


            //should start the timer
            //on the canvas as well
            //pm.gameObject.GetComponent<PlayerCanvasManager>().Show_ExtractionTimer();
            //or the timer can just start
            
        }
    }
    public void PlayerExitedExtractionZone(GameObject player)
    {
        playersInZone.Remove(player);

        if (playersInZone.Count <= 0)
        {
            playerInZone = false;
        }
    }

    public void PlayerEnteredZone_Deny(GameObject player)
    {
        if (player.GetComponent<PlayerManager>().teamNumber != team && playerInZone)
        {
            pm = player.GetComponent<PlayerManager>();


            enemyInZone = true;

            if (!enemiesInZone.Contains(player))
            {
                enemiesInZone.Add(player);
            }


        }
    }

    public void PlayerLeftZone_Deny(GameObject player)
    {
        enemiesInZone.Remove(player);

        if (playersInZone.Count <= 0)
        {
            enemyInZone = false;
        }
    }
}
