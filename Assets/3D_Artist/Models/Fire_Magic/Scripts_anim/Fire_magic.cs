using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_magic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
            GameObject.Find("Fire_renewed").GetComponent<Renderer>().enabled = false;  
            GameObject.Find("Fire_pic_effect").GetComponent<Renderer>().enabled = false;       
    }
public void OnMouseUp()
        {
            Debug.Log ("Fire");
            GameObject.Find("Fire_renewed").GetComponent<Renderer>().enabled = true;        
            GameObject.Find("Fire_pic_effect").GetComponent<Renderer>().enabled = true;       
            GameObject.Find("Fire_pic_old").GetComponent<Animation>().Play("Old_disappear");
            GameObject.Find("Fire_pic_renewed").GetComponent<Animation>().Play("New_appear_2");
            GameObject.Find("SM_Fire_pic_effect").GetComponent<Animation>().Play("Fire_magic_effect");

        }
    // Update is called once per frame
    void Update()
    {

    }
}
