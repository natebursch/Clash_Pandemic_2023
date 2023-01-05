using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive = 0;
    public int round = 0;
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    public TextMeshProUGUI roundText;

    public GameObject endScreen;
    public TextMeshProUGUI endRounds;

    public GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
            roundText.text = "" + round;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
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
    public void EndGame()
    {
        endScreen.SetActive(true);
        AudioListener.volume = 0;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;

        endRounds.text = "Rounds Sruvived: " + round;

    }

    public void ReplayButtonPressed()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public void MainMenuButtonPressed()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Pause()
    {
       Time.timeScale = 0;
       Cursor.lockState = CursorLockMode.None;
       AudioListener.volume = 0;
       pauseScreen.SetActive(true);
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
        pauseScreen.SetActive(false);
    }
}
