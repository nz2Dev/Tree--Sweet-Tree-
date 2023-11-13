using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_symbol : MonoBehaviour


{
    public GameObject[] Dark_corner;
    private bool WaterFlow = false;
    private float animationFinished = 0;

    void Start()
    { 
           GameObject.Find("SM_Fire_effect (1)").GetComponent<Renderer>().enabled = false;  
           GameObject.Find("SM_Fire_whole (2)").GetComponent<Renderer>().enabled = false;  
    }


    public void OnMouseUp()
    {
        if(Time.time > animationFinished)
        {
            if(WaterFlow)
            {
        Debug.Log ("Fire_symbol!!!");
        GameObject.Find("SM_Fire_effect (1)").GetComponent<Renderer>().enabled = true;  
        GameObject.Find("SM_Fire_whole (2)").GetComponent<Renderer>().enabled = true;   
        GameObject.Find("SM_Fire_part_1").GetComponent<Animation>().Play("Fire_part_1_disappear");
        GameObject.Find("SM_Fire_part_2 (1)").GetComponent<Animation>().Play("Fire_part_2_disappear");
        GameObject.Find("SM_Fire_whole (2)").GetComponent<Animation>().Play("Fire_whole_appear");
        GameObject.Find("SM_Fire_effect (1)").GetComponent<Animation>().Play("Fire_symbol_effect");
        GameObject.Find("Point Light 1 (17)").GetComponent<Animation>().Play("Light");
           GameObject.Find("SM_Branch_01 (1)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Curved_Wall (2)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Curved_Wall (3)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Curved_Wall (5)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (11)").GetComponent<MaterialLerp>().enabled = true;
       //    GameObject.Find("SM_Branch_04 (1)").GetComponent<MaterialLerp>().enabled = true;
         //  GameObject.Find("SM_Branch_04 (5)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_02 (10)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_02 (13)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_02 (12)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_02 (14)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_02 (15)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_04 (9)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_04 (10)").GetComponent<MaterialLerp>().enabled = true;
      //     GameObject.Find("SM_Branch_01").GetComponent<MaterialLerp>().enabled = true;
         //  GameObject.Find("SM_Branch_04").GetComponent<MaterialLerp>().enabled = true;
    //       GameObject.Find("SM_Branch_04 (3)").GetComponent<MaterialLerp>().enabled = true;
     //      GameObject.Find("SM_Branch_6 (1)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (15)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (10)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (13)").GetComponent<MaterialLerp>().enabled = true;
   //        GameObject.Find("SM_Branch_7").GetComponent<MaterialLerp>().enabled = true;
    //       GameObject.Find("SM_Branch_03 (2)").GetComponent<MaterialLerp>().enabled = true;
    //       GameObject.Find("SM_Branch_04 (2)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_7 (6)").GetComponent<MaterialLerp>().enabled = true;
    //       GameObject.Find("SM_Branch_02 (1)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_03 (1)").GetComponent<MaterialLerp>().enabled = true;
       //    GameObject.Find("SM_Branch_7 (1)").GetComponent<MaterialLerp>().enabled = true;
      //     GameObject.Find("SM_Branch_02 (1)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (13)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (13)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (13)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_01 (13)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (1)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_02").GetComponent<MaterialLerp>().enabled = true;


        }

            animationFinished = Time.time + 1;
            WaterFlow = !WaterFlow;
        }

    }


    
}

