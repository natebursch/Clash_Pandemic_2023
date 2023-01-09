using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WeaponManager : MonoBehaviour
{
    //or maybe use the muzzle? IDK
    public GameObject playerCam;
    public float bulletRange = 100f;
    public float bulletBodyDamage = 50f;
    public float bulletHeadShotDamage = 100f;
    public float friendlyFireReduction = 3;

    public Animator animator;

    public ParticleSystem muzzleFlash;
    public GameObject hitBloodParticles;
    public GameObject hitParticles;

    public AudioClip gunShot;
    public AudioSource audioSource;

    public bool isAiming;

    //public WeaponSway weaponSway;
    float swaySensitivity;
    //public float aimSensitivity = 8;

    public MouseLook mouseLook;
    public float aimSensitivity;
    public float normalSensitivity;

    public GameObject crosshair;

    public bool canFullAuto;
    public float fireRate;
    public float fireRateTimer;

    //bullet impact force
    public float bulletForce = 10000f;

    public int currentAmmo;
    public int magazineSize;
    public float reloadTime;
    public bool isReloading;
    public int reserveAmmo;

    public TextMeshProUGUI ammoText;

    public string weaponType;

    public PlayerManager playerManager;

    public PhotonView photonView;

    public float aimFOV = 25f;
    public float normalFOV = 60f;
    public float aimSpeed = .1f;

    //Start is called before the first frame update
    private void OnEnable()
    {
        animator.SetTrigger(weaponType);
        UpdateGUIText();
    }

    private void OnDisable()
    {
        //reset reloading
        UpdateGUIText();
        animator.SetBool("isReloading", false);
        isReloading = false;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //swaySensitivity = weaponSway.swaySensitivity;

        UpdateGUIText();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }

        Aim();

        if (animator.GetBool("isShooting"))
        {
            animator.SetBool("isShooting", false);
        }

        Reload();

        //count down on the timer
        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }
        //get left click

        if (Input.GetKey(KeyCode.Mouse0) && !isReloading && currentAmmo > 0 && fireRateTimer <= 0 && canFullAuto)
        {
            //Debug.Log("SHOOT");
            Shoot();
            fireRateTimer = fireRate;


        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading && currentAmmo > 0 && fireRateTimer <= 0 && !canFullAuto)
        {
            Shoot();
            fireRateTimer = fireRate;
        }

    }
    public void UpdateGUIText()
    {
        ammoText.text = currentAmmo + "/" + reserveAmmo;
    }
    void Reload()
    {
        //auto reload
        //if (reserveAmmo <= 0 && currentAmmo <= 0)
        //{
        //    return;
        //}

        //if (currentAmmo <= 0 && !isReloading)
        //{
        //    StartCoroutine(Reload(reloadTime));
        //    return;
        //}


        //if (isReloading)
        //{
        //    return;
        //}
        //manual reload
        if (Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0 && !isReloading)
        {
            StartCoroutine(Reload(reloadTime));
            return;
        }
    }

    public IEnumerator Reload(float reloadTime)
    {
        isReloading = true;
        animator.SetBool("isReloading", true);
        yield return new WaitForSeconds(reloadTime);



        int missingAmmo = magazineSize - currentAmmo;

        if (reserveAmmo >= missingAmmo)
        {
            currentAmmo += missingAmmo;
            reserveAmmo -= missingAmmo;
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }

        UpdateGUIText();
        animator.SetBool("isReloading", false);
        isReloading = false;
    }
    void Aim()
    {
        //aiming
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isAiming = true;
            animator.SetBool("isAiming", true);
            //weaponSway.swaySensitivity = aimSensitivity;
            mouseLook.mouseSensitvity = aimSensitivity;

            playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.GetComponent<Camera>().fieldOfView,aimFOV, aimSpeed);


            crosshair.SetActive(false);
        }
        else
        {
            isAiming = false;
            animator.SetBool("isAiming", false);
            //weaponSway.swaySensitivity = swaySensitivity;
            mouseLook.mouseSensitvity = normalSensitivity;

            playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.GetComponent<Camera>().fieldOfView, normalFOV, aimSpeed);


            crosshair.SetActive(true);

        }
    }
    public void ShootVFX(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            muzzleFlash.Play();
            audioSource.PlayOneShot(gunShot);
        }
    }
    void Shoot()
    {
        currentAmmo--;
        UpdateGUIText();
        //waepon effects
        animator.SetBool("isShooting", true);

        //sfx
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("WeaponShootVFX", RpcTarget.All, photonView.ViewID);
        }
        else
        {
            ShootVFX(photonView.ViewID);
        }

        //stores information in a raycast struct
        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, bulletRange))
        {
            if (hit.transform.gameObject.tag == null)
            {
                return;
            }

            //Debug.Log(hit.transform.gameObject.tag);

            //if hit a zombie
            if (hit.transform.GetComponentInParent<EnemyManager>() != null)
            {
                EnemyManager enemy = hit.transform.GetComponentInParent<EnemyManager>();
                //add force
                //hit.rigidbody.isKinematic = true;
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
                enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage,photonView.ViewID, gameObject);
                EnemyManager enemyMan = enemy.GetComponent<EnemyManager>();

                if (enemyMan.health <= 0 && !enemy.GetComponent<EnemyManager>().hasDied)
                {
                    playerManager.UpdatePoints(enemy.worthPoints);
                }
                //if (enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage))
                //{
                //    playerManager.UpdatePoints(enemy.worthPoints);
                //}
            }
            //if hit player
            else if (hit.transform.GetComponentInParent<PlayerManager>()!=null)
            {
                PlayerManager hitPlayerManger = hit.transform.GetComponentInParent<PlayerManager>();
                if (hitPlayerManger.teamNumber == gameObject.GetComponentInParent<PlayerManager>().teamNumber)
                {
                    //same team
                    Debug.Log("Do reduced damage");
                    //check if body or head but i dont have it set up atm
                    hitPlayerManger.Hit(bulletBodyDamage / friendlyFireReduction);

                }
                else
                {
                    //also need headshots here but dont have that yet
                    hitPlayerManger.Hit(bulletBodyDamage);
                }
            }
            //if hit basic zombie.... LOL i just need to make some inheritence go on, but we can do that later........ maybe
            else if(hit.transform.GetComponentInParent<ZombieBasicManager>()!= null)
            {
                ZombieBasicManager enemy = hit.transform.GetComponentInParent<ZombieBasicManager>();
                //add force
                //hit.rigidbody.isKinematic = true;
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
                enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage, photonView.ViewID, gameObject);
                ZombieBasicManager enemyMan = enemy.GetComponent<ZombieBasicManager>();

                if (enemyMan.health <= 0 && !enemy.GetComponent<ZombieBasicManager>().hasDied)
                {
                    playerManager.UpdatePoints(enemy.worthPoints);
                }
                //if (enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage))
                //{
                //    playerManager.UpdatePoints(enemy.worthPoints);
                //}
            }

            else
            {
                GameObject hitObject;

                if (PhotonNetwork.InRoom)
                {
                    hitObject = PhotonNetwork.Instantiate("HitParticles", hit.point, Quaternion.LookRotation(hit.normal));
                }
                else
                {
                    hitObject = Instantiate(Resources.Load("HitParticles"), hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;

                }
                Destroy(hitObject, .5f);

                hitObject.GetComponent<ParticleSystem>().Play();
            }

        }



    }
}
