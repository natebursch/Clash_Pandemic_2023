using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieBasicDamageElement : MonoBehaviour
{
    public Animator animator;
    public ZombieBasicManager enemyManager;

    public void OnTriggerEnter(Collider other)
    {

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            //if not a master client
            return;
        }
        Debug.Log(other);
        //might need photon pun thing to be happening here
        if (animator.GetBool("isAttacking") == true && other.gameObject.GetComponent<PlayerManager>())
        {
            Debug.Log("Attack Player");
            other.gameObject.GetComponent<PlayerManager>().Hit(enemyManager.damage);
        }
    }

}
