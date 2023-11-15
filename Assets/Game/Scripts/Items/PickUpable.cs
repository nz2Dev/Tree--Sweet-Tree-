using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUpable : MonoBehaviour {
    
    [SerializeField] private float pickUpRadius = 1f;
    [SerializeField] private Item inventoryItem;
    [SerializeField] private GameObject destructionEffectPrefab;
    [SerializeField] private JumpPlatform pickUpPlatform;
    [SerializeField] private UnityEvent OnConsumed;

    public float PickUpRadius => pickUpRadius;
    public JumpPlatform PickUpPlatform => pickUpPlatform;
    public Item InventoryItem => inventoryItem;

    public event Action OnConsumedEvent;

    public void Setup(Item item) {
        inventoryItem = item;
    }

    public void Release() {
        if (Physics.Raycast(transform.position, -Vector3.up, out var hitInfo, 10)) {
            transform.position = hitInfo.point;
        }
    }

    public void DestroySelf(bool consumed) {
        if (consumed) {
            OnConsumed?.Invoke();
            OnConsumedEvent?.Invoke();
        }
        if (consumed && destructionEffectPrefab != null) {
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
