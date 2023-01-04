using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive = 0;
    public int round = 0;
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    public TextMeshProUGUI roundText;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {

        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
            roundText.text = ""+round;
        }


    }

    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemy.GetComponent<EnemyManager>().gameManager = gameObject.GetComponent<GameManager>();

            enemiesAlive++;
        }

    }
}
