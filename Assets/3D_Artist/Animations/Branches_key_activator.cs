using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branches_key_activator : MonoBehaviour {

    private bool doorOpen = false;
    private float animationFinished = 0;


    void Start()
    {

    }

    public void OnMouseUp()
    {
        Debug.Log ("Branches");

        if(Time.time > animationFinished)
        {
            if(doorOpen)
            {
                GetComponent<Animation>().Play("Branches_key_hide");
            }


            animationFinished = Time.time + 1;
            doorOpen = !doorOpen;
        }
    }
}