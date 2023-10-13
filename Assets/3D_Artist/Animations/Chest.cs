using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    private bool doorOpen = false;
    private float animationFinished = 0;

    public void OnMouseUp()
    {
        Debug.Log ("Ive been clicked");

        if(Time.time > animationFinished)
        {
            if(doorOpen)
            {
                GetComponent<Animation>().Play("CloseChest");
            }
            else
            {
                GetComponent<Animation>().Play("OpenChest");
            }

            animationFinished = Time.time + 1;
            doorOpen = !doorOpen;
        }
    }
}