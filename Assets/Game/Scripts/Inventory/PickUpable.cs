using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpable : MonoBehaviour {
    
    [SerializeField] private float pickUpRadius = 1f;
    [SerializeField] private Item inventoryItem;
    [SerializeField] private GameObject destructionEffectPrefab;

    public float PickUpRadius => pickUpRadius;
    public Item InventoryItem => inventoryItem;

    public void Release() {
        if (Physics.Raycast(transform.position, -Vector3.up, out var hitInfo, 10)) {
            transform.position = hitInfo.point;
        }
    }

    public void DestroySelf(bool consumed) {
        if (consumed) {
            var destructionEffectInstance = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
            destructionEffectInstance.SetActive(true);
        }
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);        
    }
#endif

}
