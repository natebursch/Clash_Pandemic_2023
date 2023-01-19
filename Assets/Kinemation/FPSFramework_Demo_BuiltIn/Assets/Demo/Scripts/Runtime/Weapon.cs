// Designed by Kinemation, 2022
//edited by nate bursch. 1-19-2023

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Photon.Pun;

namespace Demo.Scripts.Runtime
{
    public class Weapon : MonoBehaviour
    {

        [Header("Weapon Settings")]
        [SerializeField] private int bulletBodyDamage = 50;
        [SerializeField] private int bulletHeadShotDamage = 200;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gunShotClip;

        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private GameObject muzzleEffect;

        //[SerializeField] private GameObject shellEjection;
        //[SerializeField] private Transform shellEjectionPort;

        [SerializeField] private Camera playerCamera;


        [SerializeField] private List<Transform> scopes;
        [SerializeField] public WeaponAnimData gunData;
        [SerializeField] public RecoilAnimData recoilData;
        
        public FireMode fireMode;
        public float fireRate;
        public int burstAmount;

        public float bulletRange = 500;
        public float bulletForce = 5f;
        private Animator _animator;
        private int _scopeIndex;


        //multiplayer
        [Header("Multiplayer Stuff")]
        public PhotonView pv;
        private void Start()
        {
            _animator = GetComponent<Animator>();

        }

        public Transform GetScope()
        {
            _scopeIndex++;
            _scopeIndex = _scopeIndex > scopes.Count - 1 ? 0 : _scopeIndex;
            return scopes[_scopeIndex];
        }
        
        public void OnFire()
        {
            PlayFireAnim();
        }

        private void WeaponFX()
        {
            Debug.Log("Why");
            audioSource.PlayOneShot(gunShotClip);
            //play the muzzleflash
            GameObject muzzleFlash = Instantiate(muzzleEffect, muzzleTransform.position, muzzleTransform.rotation);
            Destroy(muzzleFlash, 1f);
        }
        private void PlayFireAnim()
        {
            if (_animator != null)
            {
                _animator.Play("Fire", 0, 0f);
            }


            //play the sound
            WeaponFX();
            //shoot a raycast bullet
            Shoot();
        }

        private void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, bulletRange))
            {
                if (hit.transform.gameObject.tag == null) { return; }
                string hitTag = hit.transform.gameObject.tag;

                Debug.Log(hitTag);

                //hit a zombie
                if (hit.transform.gameObject.GetComponentInParent<ZombieBasicManager>()!= null)
                {
                    ZombieBasicManager enemy = hit.transform.GetComponentInParent<ZombieBasicManager>();
                    hit.rigidbody.AddForceAtPosition(-transform.TransformDirection(Vector3.forward) * bulletForce, hit.point);

                    GameObject hitObject;

                    if (PhotonNetwork.InRoom)
                    {
                        hitObject = PhotonNetwork.Instantiate("BloodHitEffect", hit.point, Quaternion.LookRotation(hit.normal));
                    }
                    else
                    {
                        hitObject = Instantiate(Resources.Load("BloodHitEffect"), hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;

                    }
                    Destroy(hitObject, .5f);
                    hitObject.GetComponent<ParticleSystem>().Play();
                    //hitObject.transform.position = hit.transform.position;

                    //enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage);
                    //if enemy health is < 0 add points
                    enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage, pv.ViewID, gameObject);
                    ZombieBasicManager enemyMan = enemy.GetComponent<ZombieBasicManager>();

                    //FOR POINTS

                    //if (enemyMan.health <= 0 && !enemy.GetComponent<ZombieBasicManager>().hasDied)
                    //{
                    //    playerManager.UpdatePoints(enemy.worthPoints);
                    //}
                    //if (enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage))
                    //{
                    //    playerManager.UpdatePoints(enemy.worthPoints);
                    //}
                }




            }
           }




        }
    }

