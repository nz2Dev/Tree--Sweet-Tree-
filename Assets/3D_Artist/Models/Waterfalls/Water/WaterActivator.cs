using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterActivator : MonoBehaviour


{

    private bool WaterFlow = false;
    private float animationFinished = 0;

    void Start()
    {
           GameObject.Find("SM_Tree_03 (11)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (3)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (2)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (4)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (9)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (8)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (7)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (6)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Tree_03 (5)").GetComponent<MaterialLerp>().enabled = false;
           GameObject.Find("SM_Branch_03").GetComponent<MaterialLerp>().enabled = false;
    }


    public void OnMouseUp()
    {
        Debug.Log ("WATER");

        if(Time.time > animationFinished)
        {
            if(WaterFlow)
            {
                GetComponent<Animation>().Play("Water_flow");
           GameObject.Find("SM_Tree_03 (11)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (3)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (2)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (4)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (9)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (8)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (7)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (6)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Tree_03 (5)").GetComponent<MaterialLerp>().enabled = true;
           GameObject.Find("SM_Branch_03").GetComponent<MaterialLerp>().enabled = true;
            }

            animationFinished = Time.time + 1;
            WaterFlow = !WaterFlow;
        }
    }
}

