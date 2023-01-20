using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCrosshair : MonoBehaviour
{
    private RectTransform reticle; // The RecTransform of reticle UI element.

    public FPSController controller;

    public float restingSize;
    public float crouchSize;
    public float walkSize;
    public float sprintSize;
    public float speed;
    private float currentSize;

    public GameObject[] crosshairTicks;

    private void Start()
    {

        reticle = GetComponent<RectTransform>();

    }

    private void Update()
    {
        //aint no way this is runs well
        if (controller._aiming)
        {
            foreach (GameObject tick in crosshairTicks)
            {
                tick.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject tick in crosshairTicks)
            {
                tick.SetActive(true);
            }
        }
        // Check if player is currently moving and Lerp currentSize to the appropriate value.
        if (controller._crouching)
        {
            currentSize = Mathf.Lerp(currentSize,crouchSize , Time.deltaTime * speed);
        }
        else if(controller._sprinting)
        {
            currentSize = Mathf.Lerp(currentSize, sprintSize, Time.deltaTime * speed);
        }
        else if (controller.GetComponent<CharacterController>().velocity.sqrMagnitude > 0)
        {
            currentSize = Mathf.Lerp(currentSize, walkSize, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);

        }



        // Set the reticle's size to the currentSize value.
        reticle.sizeDelta = new Vector2(currentSize, currentSize);

    }

}
