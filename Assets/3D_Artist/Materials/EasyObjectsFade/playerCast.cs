using UnityEngine;

public class playerCast : MonoBehaviour
{
    public float radius;
    public float maxDistance;
    public LayerMask layerMask;
    RaycastHit[] hits;

    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {   
            Cast();
        }
        
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawSphere(transform.position-transform.up*maxDistance,radius);
    }
    void Cast()
    {
        hits=Physics.SphereCastAll(transform.position,radius,-transform.up,maxDistance,~layerMask);
        Debug.Log(hits.Length);
        Debug.Log ("AAAAAAAAAAAAAAAAAA");
        
    }
}