using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHide : MonoBehaviour
{

    public Transform playerTransform;
    public GameObject currentBuilding;
    public float distCalculate;

    Ray castRay;
    RaycastHit castHit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        castRay = new Ray(Camera.main.transform.position, playerTransform.position - Camera.main.transform.position);

        distCalculate = Vector3.Distance(playerTransform.position,Camera.main.transform.position);

        if(Physics.Raycast(castRay,out castHit, distCalculate))
        {
            if (castHit.collider != null)
            {
                if (castHit.collider.CompareTag("Buildings"))
                {
                    if (currentBuilding != null)
                    {
                        if (currentBuilding != castHit.collider.gameObject)
                        {
                            enableBuilding(true);
                            currentBuilding = castHit.collider.gameObject;
                            enableBuilding(false);
                        }
                    }
                    else
                    {
                        currentBuilding = castHit.collider.gameObject;
                        enableBuilding(false);
                    }
                }
                else
                {
                    if (currentBuilding != null)
                    {
                        enableBuilding(true);
                        currentBuilding = null;
                    }
                }
            }
        }
    }

    public void enableBuilding(bool boolOperation)
    {
        if(currentBuilding.transform.parent!=null && currentBuilding.transform.parent.CompareTag("Buildings"))
        {
            for(int i = 0; i < currentBuilding.transform.parent.childCount; i++)
            {
                MeshRenderer buildingMesh = currentBuilding.transform.parent.GetChild(i).GetComponent<MeshRenderer>() ?? null;

                if (buildingMesh != null)
                {
                    buildingMesh.enabled = boolOperation;
                }
            }
        }
        else if (currentBuilding.transform.childCount < 0)
        {
            for(int i = 0; i < currentBuilding.transform.childCount; i++)
            {
                MeshRenderer buildinMesh = currentBuilding.transform.GetChild(i).GetComponent<MeshRenderer>() ?? null;
                if (buildinMesh != null)
                {
                    buildinMesh.enabled = boolOperation;
                }
            }
        }
        else
        {
            MeshRenderer buildingMesh = currentBuilding.GetComponent<MeshRenderer>() ?? null;
            if (buildingMesh != null)
            {
                buildingMesh.enabled = boolOperation;
            }
        }
    }
}