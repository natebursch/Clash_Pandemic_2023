using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;



public class ZombieBasicManager : MonoBehaviourPunCallbacks
{
    

    public GameObject player;
    public Animator animator;
    public float damage = 20f;
    public float health = 100f;
    public BossRoomManager gameManager;
    public bool inBossRoom = false;

    public float zombieAttackRange = 2;
    public bool playerInReach;
    public float attackDelayTimer;

    public float attackDelay = .5f;
    public float attackAnimStartDelay;
    public float delayBetweenAttacks;

    public AudioSource audioSource;
    public AudioClip[] zombieSounds;
    public AudioClip[] deathSounds;
    public bool hasDied;

    public float worthPoints = 20;

    public GameObject[] players;

    public PhotonView photonView;

    public float attackDistance = 25;
    public float randomDistance = 5;
    public bool isClose = false;

    public float runningSpeed = 5.5f;
    public float walkingSpeed = 2f;
    public bool walkingRandomPlace = false;
    // Start is called before the first frame update
    void Start()
    {
        hasDied = false;
        players = GameObject.FindGameObjectsWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();


        audioSource = GetComponent<AudioSource>();

        setRigidbodyState(true);
        setColliderState(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && health > 0)
        {
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            //if not a master client
            return;
        }

        //find the closest player


        /// HERE IS HOW TO FIXZ THE ASLGASGAGAF LAG YOU DUMBy
        /// INSTEAD OF CALCULATING THIS AT ALL TIMES AND CALCULATING ALL THIS JUNK,
        /// MAKE A COLLIDER THAT IS THE RANGE OF THE ZOMBIE
        /// AND THEN ONLY WHEN A PLAYER ENTERS THE RANGE ARE THEY INCLUDED IN THE CALCULATION
        /// AND ONLY CALCULATE THOSE PLAYERS IN DISTANCE
        /// AND THEN YOU CAN START THE NAVIGATION PROCESS
        /// OTHERWISE JUST LET THEM ROAM AROUND
        /// OR DO NOTHING
        /// IN FACE
        /// DONT EVEN HAVE THEM SPAWN ALL THERE MATERIALS AND SHTUFF
        /// 
        float distance = GetClosestPlayer();

        if (player != null)
        {
            if (health > 0)
            {



                //if allowed to chase
                if (distance < attackDistance)
                {
                    isClose = true;
                    GetComponent<NavMeshAgent>().destination = player.transform.position;

                    GetComponent<NavMeshAgent>().speed = runningSpeed;

                    //if running
                    if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
                    {

                        animator.SetBool("isRunning", true);
                        animator.SetBool("isWalking", false);
                        animator.SetBool("isAttacking", false);




                    }
                    else
                    {
                        animator.SetBool("isRunning", false);
                        animator.SetBool("isWalking", false);

                        animator.SetBool("isAttacking", true);
                    }
                }
                else
                {
                    if (walkingRandomPlace)
                    {
                        Vector3 curPos = transform.position;
                        float distanceToRAndomPoint = Vector3.Distance(GetComponent<NavMeshAgent>().destination, curPos);
                        if (distanceToRAndomPoint > 1.75f)
                        {
                            return;
                        }

                        walkingRandomPlace = false;
                    }
                    walkingRandomPlace = true;
                    isClose = false;
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isAttacking", false);

                    GetComponent<NavMeshAgent>().speed = walkingSpeed;

                    Vector3 patrolPoint = RandNavMeshLocation();
                    GetComponent<NavMeshAgent>().destination = patrolPoint;
                }

            }
            if (health <= 0)
            {
                return;
            }



        }



    }
    public Vector3 RandNavMeshLocation()
    {

        Vector3 pos = Vector3.zero;
        Vector3 randomPos = Random.insideUnitSphere * randomDistance;
        randomPos += transform.position;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, randomDistance, 1))
        {
            pos = hit.position;
        }
        return pos;

    }

    private float GetClosestPlayer()
    {
        float minDistance = Mathf.Infinity;
        Vector3 curPos = transform.position;

        foreach (GameObject thisPlayer in players)
        {
            if (thisPlayer != null)
            {
                float distance = Vector3.Distance(thisPlayer.transform.position, curPos);

                if (distance < minDistance)
                {
                    player = thisPlayer;
                    minDistance = distance;
                }
            }
        }
        return minDistance;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log(other);
    //    if (other.gameObject == player)
    //    {
    //        playerInReach = true;

    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (playerInReach)
    //    {
    //        attackDelayTimer += Time.deltaTime;
    //    }
    //    if (attackDelayTimer >= delayBetweenAttacks - attackAnimStartDelay && attackDelayTimer <= delayBetweenAttacks && playerInReach)
    //    {
    //        animator.SetTrigger("isAttacking");
    //    }
    //    if (attackDelayTimer >= delayBetweenAttacks && playerInReach)
    //    {
    //        Debug.Log("Do Damage xZobmie");
    //        player.GetComponent<PlayerManager>().Hit(damage);
    //        attackDelayTimer = 0;
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject == player)
    //    {
    //        playerInReach = false;
    //        animator.SetBool("isAttacking",false);
    //        attackDelayTimer = 0;
    //    }
    //}

    public void Hit(float damage, int shooterID, GameObject attacker)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("TakeDamage", RpcTarget.All, damage, shooterID, photonView.ViewID);
        }
        else
        {

            health -= damage;
            if (health <= 0)
            {
                audioSource.Stop();
                animator.enabled = false;
                setRigidbodyState(false);
                setColliderState(true);


                if (!hasDied)
                {
                    if (inBossRoom)
                    {
                        Debug.Log("bruh what why");
                        gameManager.enemiesAlive--;
                    }

                    Destroy(GetComponent<NavMeshAgent>());
                    Destroy(gameObject, 5f);
                    hasDied = true;


                    //attempt to add points to the shooter
                    attacker.GetComponentInParent<PlayerManager>().UpdatePoints(worthPoints);


                }
                else
                {
                    hasDied = true;

                }

            }


        }

    }

    [PunRPC]
    public void TakeDamage(float damage, int shooterID, int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            health -= damage;
            if (health <= 0)
            {
                audioSource.Stop();
                animator.enabled = false;
                setRigidbodyState(false);
                setColliderState(true);


                if (!hasDied)
                {
                    if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
                    {

                    }
                    if (inBossRoom)
                    {
                        Debug.Log("bruh what why");
                        gameManager.enemiesAlive--;
                    }

                    Destroy(GetComponent<NavMeshAgent>());
                    Destroy(gameObject, 5f);
                    hasDied = true;


                    //attempt to add points to the shooter
                    PhotonView.Find(shooterID).gameObject.GetComponent<PlayerManager>().UpdatePoints(worthPoints);

                }
                else
                {
                    hasDied = true;

                }

            }

        }

    }

    //public bool Die()
    //{

    //    //audioSource.Stop();
    //    //animator.enabled = false;
    //    //setRigidbodyState(false);
    //    //setColliderState(true);


    //    //if (!hasDied)
    //    //{
    //    //    gameManager.enemiesAlive--;
    //    //    Destroy(gameObject, 5f);
    //    //    hasDied = true;
    //    //    return true;
    //    //}
    //    //else
    //    //{
    //    //    hasDied = true;
    //    //    return false;
    //    //}



    //}

    void setRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
        GetComponent<Rigidbody>().isKinematic = !state;
    }
    void setColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            //make sure the colllider is tagged
            if (collider.gameObject.tag != "Head" && collider.gameObject.tag != "AttackHand")
            {
                //Debug.Log(collider.gameObject.tag);
                collider.enabled = state;
            }

        }
        if (GetComponent<Collider>()!=null)
        {
            GetComponent<Collider>().enabled = !state;
        }
        

    }
}
