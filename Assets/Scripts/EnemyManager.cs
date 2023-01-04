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


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<NavMeshAgent>().destination = player.transform.position;

        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("PlayerHit");
            player.GetComponent<PlayerManager>().Hit(damage);
        }
    }

    public void Hit(float damage)
    {
        Debug.Log(damage);
        health -= damage;
        Debug.Log("Enemy Health " + health);
        if (health<= 0)
        {
            gameManager.enemiesAlive--;
            Destroy(gameObject);
        }
    }

}
