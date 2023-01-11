using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bounty_Reward : MonoBehaviour
{

    public BossRoomManager bossRoomManager;

    public PlayerManager playerManager;


    public bool PlayerInReach = false;

    public string canvas_interactable_text = "Press E to pickup";

    Collider player;

    public PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        bossRoomManager = FindObjectOfType<BossRoomManager>();
    }

    private void Update()
    {
        if (PlayerInReach &&  Input.GetKeyDown(KeyCode.E))
        {
            bossRoomManager.ShowMissionStatus(bossRoomManager.bountyHasBeenGrabbed_Announcement, bossRoomManager.bountyHasBeenGrabbed_tooltip);

            //place a gameobject on the player that pressed e
            player.gameObject.GetComponent<PlayerManager>().ShowHat(true);

            player.gameObject.GetComponent<PlayerCanvasManager>().Hide_InteractbleText();
            PlayerInReach = false;
            //destroy bounty
            ///need to send an rpc here
            Destroy_BountyReward();

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerInReach = true;
            player = other;
            other.gameObject.GetComponent<PlayerCanvasManager>().Show_InteractbleText(canvas_interactable_text);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerCanvasManager>().Hide_InteractbleText();
            PlayerInReach = false;
        }
    }
    
    public void Destroy_BountyReward()
    {
        if (PhotonNetwork.InRoom)
        {
            pv.RPC("RPC_Destroy_BountyReward",RpcTarget.All);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [PunRPC]
    public void RPC_Destroy_BountyReward()
    {
        Destroy(gameObject);
    }


}
