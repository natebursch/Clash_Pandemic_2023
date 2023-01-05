using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{


    public float price;
    public string doText = "Press E";
    public Text priceNumberText;
    public Text actionText;
    public Text typeText;

    PlayerManager playerManager;

    public bool playerInReach = false;

    public Type shopType;
    public enum Type { Health,Ammo}
    public float productAmount;


    // Start is called before the first frame update
    void Start()
    {
        priceNumberText.text = price.ToString();
        actionText.text = doText;
        typeText.text = ""+ shopType;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //turn on vending machine
            actionText.text = doText;

            priceNumberText.gameObject.SetActive(true);
            actionText.gameObject.SetActive(true);

            playerInReach = true;

            playerManager = other.GetComponent<PlayerManager>();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //turn on vending machine
            priceNumberText.gameObject.SetActive(false);
            actionText.gameObject.SetActive(false);

            playerInReach = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInReach && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Penis");
            BuyShop();
        }
    }

    void BuyShop()
    {
        if (playerManager.currentPoints >= price)
        {

            if (shopType == Type.Health)
            {
                playerManager.UpdateHealth(productAmount);
            }
            else if (shopType == Type.Ammo)
            {
                playerManager.UpdateAmmo((int)productAmount);
            }




            playerManager.UpdatePoints(-price);
        }
        else
        {
            actionText.text = "Not Enough Funds";
        }

    }
}
