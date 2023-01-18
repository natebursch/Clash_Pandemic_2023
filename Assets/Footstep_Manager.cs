using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Footstep_Manager : MonoBehaviour
{

    private AudioSource audioSource;
    private PhotonView pv;

    public AudioClip[] footsteps;


    // Start is called before the first frame update
    void Start()
    {
        pv = gameObject.GetComponentInParent<PhotonView>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play_Footstep_FX()
    {

        //might be able to just make all of this local
        audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);

        //if (PhotonNetwork.InRoom)
        //{
        //    pv.RPC("RPC_Play_FootStep_FX",RpcTarget.All, pv.ViewID);
        //}
        //else
        //{
        //    audioSource.PlayOneShot(footsteps[Random.Range(0,footsteps.Length)]);
        //}
    }

    [PunRPC]
    public void RPC_Play_FootStep_FX(int id)
    {
        if (id == pv.ViewID)
        {
            audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
        }
    }
}
