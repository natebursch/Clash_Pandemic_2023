using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //or maybe use the muzzle? IDK
    public GameObject playerCam;
    public float bulletRange = 100f;
    public float bulletBodyDamage = 50f;
    public float bulletHeadShotDamage = 100f;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("isShooting"))
        {
            animator.SetBool("isShooting", false);
        }

        //get left click
        if (Input.GetButtonDown("Fire1"))
        {
            //Debug.Log("SHOOT");
            Shoot();


        }
    }

    void Shoot()
    {
        //waepon effects
        animator.SetBool("isShooting", true);

        //stores information in a raycast struct
        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, bulletRange))
        {
            if (hit.transform.gameObject.tag != null)
            {
                Debug.Log(hit.transform.gameObject.tag);
            }

           
            if (hit.transform.GetComponentInParent<EnemyManager>() != null)
            {
                EnemyManager enemy = hit.transform.GetComponentInParent<EnemyManager>();
                enemy.Hit(hit.transform.gameObject.tag == "Head" ? bulletHeadShotDamage : bulletBodyDamage);
            }
        }



    }
}
