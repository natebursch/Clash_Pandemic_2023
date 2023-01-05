using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public Animator animator;
    public float damage = 20f;
    public float health = 100f;
    public GameManager gameManager;

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
    // Start is called before the first frame update
    void Start()
    {
        hasDied = false;
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();

        setRigidbodyState(true);
        setColliderState(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && health > 0)
        {
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }

        if (health > 0)
        {
            GetComponent<NavMeshAgent>().destination = player.transform.position;
        }
        if (health < 0)
        {
            GetComponent<NavMeshAgent>().destination = transform.position;
        }


        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
        {
            
            animator.SetBool("isRunning", true);
            //animator.SetBool("isAttacking", false);

        }
        else
        {
            animator.SetBool("isRunning", false);
            //animator.SetBool("isAttacking", true);
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.gameObject == player)
        {
            playerInReach = true;

        }
    }
 
    private void OnTriggerStay(Collider other)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
        }
        if (attackDelayTimer >= delayBetweenAttacks - attackAnimStartDelay && attackDelayTimer <= delayBetweenAttacks && playerInReach)
        {
            animator.SetTrigger("isAttacking");
        }
        if (attackDelayTimer >= delayBetweenAttacks && playerInReach)
        {
            Debug.Log("Do Damage xZobmie");
            player.GetComponent<PlayerManager>().Hit(damage);
            attackDelayTimer = 0;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInReach = false;
            animator.SetBool("isAttacking",false);
            attackDelayTimer = 0;
        }
    }

    public bool Hit(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            return Die();

        }
        else { return false; }
    }

    public bool Die()
    {
        
        audioSource.Stop();
        animator.enabled = false;
        setRigidbodyState(false);
        setColliderState(true);


        if (!hasDied)
        {
            gameManager.enemiesAlive--;
            Destroy(gameObject, 5f);
            hasDied = true;
            return true;
        }
        else
        {
            hasDied = true;
            return false;
        }

        
        
    }

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
            if (collider.gameObject.tag != "Head")
            {
                collider.enabled = state;
            }

        }
        GetComponent<Collider>().enabled = !state;

    }
}
