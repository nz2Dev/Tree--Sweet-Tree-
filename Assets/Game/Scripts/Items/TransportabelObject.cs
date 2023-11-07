using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableObject : MonoBehaviour {
    
    [SerializeField] private Transform hostOffset;

    public Transform Offsets => hostOffset;

}
