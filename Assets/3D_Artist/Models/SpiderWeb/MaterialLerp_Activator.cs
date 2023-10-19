using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialLerp_Activator : MonoBehaviour
{

    void Start()
    {
           GetComponent<MaterialLerp>().enabled = false;
    }


    private void OnMouseUp()
    {
           Debug.Log ("SPIDEEEEEEERRRRRRR");
           GetComponent<MaterialLerp>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
