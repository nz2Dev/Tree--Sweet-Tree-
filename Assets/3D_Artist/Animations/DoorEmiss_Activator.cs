using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEmiss_Activator : MonoBehaviour {

    private bool doorOpen = false;
    private float animationFinished = 0;


    void Start()
    {
           GetComponent<Renderer>().enabled = false;
    }

    public void OnMouseUp()
    {
        Debug.Log ("Light");
        GetComponent<Renderer>().enabled = true;

        if(Time.time > animationFinished)
        {
            if(doorOpen)
            {
                GetComponent<Animation>().Play("DoorEmissive");
            }


            animationFinished = Time.time + 1;
            doorOpen = !doorOpen;
        }
    }
}