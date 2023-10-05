using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoor : MonoBehaviour {

    private bool doorOpen = false;
    private float animationFinished = 0;

    public void OnMouseUp()
    {
        Debug.Log ("Ive been clicked");

        if(Time.time > animationFinished)
        {
            if(doorOpen)
            {
                GetComponent<Animation>().Play("CloseMainDoor");
            }
            else
            {
                GetComponent<Animation>().Play("OpenMainDoor");
            }

            animationFinished = Time.time + 1;
            doorOpen = !doorOpen;
        }
    }
}