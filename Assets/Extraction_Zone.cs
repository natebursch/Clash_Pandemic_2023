using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extraction_Zone : MonoBehaviour
{
    public Extraction_Point ex_point;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ex_point.PlayerEnteredExtractionZone(other.transform.root.gameObject);

        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ex_point.PlayerExitedExtractionZone(other.transform.root.gameObject);

        }

    }
}
