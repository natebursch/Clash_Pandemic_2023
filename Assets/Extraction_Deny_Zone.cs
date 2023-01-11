using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extraction_Deny_Zone : MonoBehaviour
{
    public Extraction_Point ex_point;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ex_point.PlayerEnteredZone_Deny(other.transform.root.gameObject);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ex_point.PlayerLeftZone_Deny(other.transform.root.gameObject);
        }
        
    }

}
