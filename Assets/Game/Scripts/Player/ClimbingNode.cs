using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingNode : MonoBehaviour {
    
    [SerializeField] private JumpPlatform hopPlatform;
    [SerializeField] private MovePlatform movePlatform;
    [SerializeField] private JumpPlatform dropPlatform;

    public JumpPlatform HopPlatform => hopPlatform;
    public MovePlatform MovePlatform => movePlatform;
    public JumpPlatform DropPlatform => dropPlatform;

}
