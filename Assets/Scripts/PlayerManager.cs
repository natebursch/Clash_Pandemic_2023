using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{

    public float health = 100;
    public TextMeshProUGUI playerHealth;

    public GameManager gameManager;

    public Camera playerCamera;
    public float shakeTime;
    public float shakeDuration;
    public Quaternion playerCameraOriginalRotation;

    public CanvasGroup hurtPanel;

    // Start is called before the first frame update
    void Start()
    {
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
        hurtPanel.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }
        if (shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        else if(playerCameraOriginalRotation != playerCamera.transform.localRotation)
        {
            playerCamera.transform.localRotation = playerCameraOriginalRotation;
        }
    }

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }

    public void Hit(float damage)
    {
        health -= damage;


        //restart game
        if (health <= 0)
        {
            health = 0;
            gameManager.EndGame();
        }
        else
        {
            shakeTime = 0;
            shakeDuration = 0.2f;
            playerHealth.text = "" + health;
            hurtPanel.alpha = 1;
        }


    }
}
