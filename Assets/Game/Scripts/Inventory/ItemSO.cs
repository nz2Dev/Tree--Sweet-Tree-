using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject {
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject dropPrefab;

    public Sprite Icon => icon;
    public GameObject TargetPrefab => targetPrefab;
    public GameObject DropPrefab => dropPrefab;
}
