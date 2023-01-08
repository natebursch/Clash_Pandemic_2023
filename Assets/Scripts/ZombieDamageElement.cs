using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDamageElement : MonoBehaviour
{
    public Animator animator;
    public EnemyManager enemyManager;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (animator.GetBool("isAttacking") == true && other.gameObject.GetComponent<PlayerManager>())
        {
            Debug.Log("Attack Player");
            other.gameObject.GetComponent<PlayerManager>().Hit(enemyManager.damage);
        }
    }

}
