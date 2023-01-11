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
    public float timer;
    public string timerText;

    public int timeToExtract;
    public int timeToExtractWithEnemy;

    //team in zone with bounty
    public List<GameObject> playersInZone;
    public int team = -1;

    PlayerManager pm;


    private void Update()
    {
        if (enemyInZone)
        {
            timer = 0;
        }
        if (playerInZone && !enemyInZone)
        {
            timer = Time.deltaTime;
        }

        if (timer >= timeToExtract)
        {
            //end the game
        }
    }
    public void PlayerEnteredExtractionZone(GameObject player)
    {
        if (player.GetComponent<PlayerManager>().hasBounty)
        {
            pm = player.GetComponent<PlayerManager>();

            team = pm.teamNumber;

            playerInZone = true;

            playersInZone.Add(player);


            //should start the timer
            
        }
    }
    public void PlayerExitedExtractionZone(GameObject player)
    {
        
    }

    public void PlayerEnteredZone_Deny(GameObject player)
    {

    }

    public void PlayerLeftZone_Deny(GameObject player)
    {

    }
}
