using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Mush_activator : MonoBehaviour
{

Animator anim;



    public void OnMouseUp()
    {
        Debug.Log ("Ive been clicked");
        anim = gameObject.GetComponent<Animator>();
        {
            anim.SetTrigger("Active");
        }

    }

//Needed to be changed for shells with water:
    public void Update()
    {
    if (Input.GetMouseButtonDown(1))
    {
        Debug.Log ("Click!");
        anim = gameObject.GetComponent<Animator>();
        {
            anim.SetTrigger("Give_water");
        }

    }
    }    

}


