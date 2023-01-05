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
        if (PhotonNetwork.InRoom && photonView.IsMine)
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

            crosshair.SetActive(false);
        }
        else
        {
            isAiming = false;
            animator.SetBool("isAiming", false);
            //weaponSway.swaySensitivity = swaySensitivity;
            mouseLook.mouseSensitvity = normalSensitivity;


            crosshair.SetActive(true);

        }
    }
    void Shoot()
    {
        currentAmmo--;
        UpdateGUIText();
        //waepon effects
        animator.SetBool("isShooting", true);

        //particle system
        muzzleFlash.Play();
        audioSource.PlayOneShot(gunShot);

        //stores information in a raycast struct
        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, bulletRange))
        {
            if (hit.transform.gameObject.tag == null)
            {
                return;
            }

            Debug.Log(hit.transform.gameObject.tag);

            if (hit.transform.GetComponentInParent<EnemyManager>() != null)
            {
                EnemyManager enemy = hit.transform.GetComponentInParent<EnemyManager>();



                //add force
                //hit.rigidbody.isKinematic = true;
                hit.rigidbody.AddForceAtPosition(-transform.TransformDirection(Vector3.forward) * bulletForce, hit.point);

                GameObject hitObject = Instantiate(hitBloodParticles, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitObject, .5f);

                hitObject.GetComponent<ParticleSystem>().Play();
                //hitObject.transform.position = hit.transform.position;

                //enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage);
                //if enemy health is < 0 add points
                if (enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage))
                {
                    playerManager.UpdatePoints(enemy.worthPoints);
                }
            }
            else
            {
                GameObject hitObject = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitObject, .5f);

                hitObject.GetComponent<ParticleSystem>().Play();
            }
        }



    }
}
