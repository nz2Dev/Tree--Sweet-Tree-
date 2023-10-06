using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cinemachine;
using UnityEngine;

public class CameraZoomController : MonoBehaviour {
    private CinemachineCameraOffset cameraOffset;
    private CinemachineFreeLook freeLook;

    [SerializeField] private float scrollSpeed = 0.5f;

    private void Awake() {
        cameraOffset = GetComponent<CinemachineCameraOffset>();
        freeLook = GetComponent<CinemachineFreeLook>();
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    public float GetAxisCustom(string axisName){
        if(axisName == "Mouse X"){
            if (Cursor.lockState == CursorLockMode.Locked){
                return UnityEngine.Input.GetAxis("Mouse X");
            } else{
                return 0;
            }
        }
        else if (axisName == "Mouse Y"){
            if (Cursor.lockState == CursorLockMode.Locked){
                return UnityEngine.Input.GetAxis("Mouse Y");
            } else{
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
    }

    private void Update() {
        if (Input.GetKey(KeyCode.LeftAlt)) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }

        var scrollDelta = Input.mouseScrollDelta.y;
        if (CinemachineCore.Instance.IsLive(freeLook)) {
            cameraOffset.m_Offset = new Vector3(cameraOffset.m_Offset.x, cameraOffset.m_Offset.y, cameraOffset.m_Offset.z + scrollDelta * scrollSpeed);
        }
    }
}
