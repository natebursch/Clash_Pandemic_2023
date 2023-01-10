using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;


public class PlayerManager : MonoBehaviourPunCallbacks
{
    public float maxHealth = 150f;
    public float health = 100;
    public TextMeshProUGUI playerHealth;

    public GameManager gameManager;

    public Camera playerCamera;
    public float shakeTime;
    public float shakeDuration;
    public Quaternion playerCameraOriginalRotation;

    public CanvasGroup hurtPanel;

    public GameObject[] weaponHolder;
    public int activeWeapon;
    public int lastWeapon;

    public float currentPoints;
    public TextMeshProUGUI pointsText;

    public int teamNumber;



    public PhotonView photonView;


    // Start is called before the first frame update
    private void Awake()
    {
        teamNumber = FindObjectOfType<Pandemic_RoomManager>().teamNumber;
    }
    void Start()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            playerCamera.gameObject.SetActive(false);
        }

        playerCameraOriginalRotation = playerCamera.transform.localRotation;

        hurtPanel.alpha = 0;
        hurtPanel.gameObject.SetActive(true);

        //make sure points are reset
        UpdatePoints(0);

        //make sure only primary weapon is showing
        WeaponSwitch(0, 0);
        activeWeapon = 0;


    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            //playerCamera.gameObject.SetActive(false); //MOVED TO START METHOD
            return;
        }
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

        //returns value between -1,1
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //then we want to choose the first weapon
            if (activeWeapon == weaponHolder.Length - 1)
            {
                WeaponSwitch(0, activeWeapon);
            }
            else
            {
                WeaponSwitch(activeWeapon + 1, activeWeapon);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //then we want to choose the last weapon
            if(activeWeapon == 0)
            {
                WeaponSwitch(weaponHolder.Length - 1, activeWeapon);
            }
            else
            {
                WeaponSwitch(activeWeapon -1, activeWeapon);
            }
        }

    }
    public void WeaponSwitching()
    {

    }

    public void UpdateHealth(float points)
    {
        Debug.Log(points);
        if (health + points > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += points;
        }

        //restart game
        if (health <= 0)
        {
            health = 0;
            //make player notmovable
            //should probably also delete the player body idk we can just freeze it
            //gameObject.GetComponent<PlayerCanvasManager>().DisableCharacter(true);
            

            gameManager.EndGame();
            gameObject.GetComponentInChildren<GameManager>().MainMenuButtonPressed();
        }

        if (points > 0)
            //added hp
        {
            shakeTime = 0;
            shakeDuration = 0.2f;

        }
        else //takes dmg
        {
            shakeTime = 0;
            shakeDuration = 0.2f;
            hurtPanel.alpha = 1;
        }

        playerHealth.text = "" + health;

    }
    public void UpdateAmmo(int amount)
    {
        weaponHolder[activeWeapon].GetComponent<WeaponManager>().reserveAmmo += amount;
        weaponHolder[activeWeapon].GetComponent<WeaponManager>().UpdateGUIText();
    }
    public void UpdatePoints(float points)
    {
        currentPoints += points;
        pointsText.text = "Points: " + currentPoints.ToString();
    }
    public void WeaponSwitch(int weaponToActivate, int currentWeapon)
    {
        //first deactivate current weapon
        weaponHolder[currentWeapon].SetActive(false);

        //activate desired weapon
        weaponHolder[weaponToActivate].SetActive(true);

        activeWeapon = weaponToActivate;

        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("weaponIndex", weaponToActivate);
            hash.Add("currentWeapon", currentWeapon);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }


    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner && changedProps["weaponIndex"]!= null && changedProps["currentWeapon"]!=null)
        {
            WeaponSwitch((int)changedProps["weaponIndex"], (int)changedProps["currentWeapon"]);
        }
    }
    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }

    public void Hit(float damage)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("PlayerRPCHit", RpcTarget.All, damage, photonView.ViewID);
        }
        else
        {
            PlayerRPCHit(damage,photonView.ViewID);
        }

    }

    [PunRPC]
    public void PlayerRPCHit(float damage, int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            UpdateHealth(-damage);
        }
    }

    [PunRPC]
    public void WeaponShootVFX(int viewID)
    {
        weaponHolder[activeWeapon].GetComponent<WeaponManager>().ShootVFX(viewID);
    }


}
