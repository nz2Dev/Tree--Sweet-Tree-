using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;

    public Transform Start => start;
    public Transform End => end;
}
