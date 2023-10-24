using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Cinemachine;
using UnityEngine;

public class CameraZoomController : MonoBehaviour {
    private CinemachineCameraOffset cameraOffset;
    private CinemachineFreeLook freeLook;

    [SerializeField] private bool useOffsetScroll = false;
    [SerializeField] private float offsetScrollSpeed = 0.5f;
    [SerializeField] private float orbitScaleScrollSpeed = 0.01f;
    [SerializeField] private float orbitScaleMin = 0.2f;

    private CinemachineFreeLook.Orbit[] initOrbits;
    private float zoomLevel = 1f;

    private void Awake() {
        cameraOffset = GetComponent<CinemachineCameraOffset>();
        freeLook = GetComponent<CinemachineFreeLook>();
        CinemachineCore.GetInputAxis = GetAxisCustom;
        initOrbits = freeLook.m_Orbits.ToArray();
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
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetMouseButton(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        var scrollDelta = Input.mouseScrollDelta.y;
        if (CinemachineCore.Instance.IsLive(freeLook)) {
            if (useOffsetScroll) {
                var zoomOffset = MathF.Max(0, cameraOffset.m_Offset.z + scrollDelta * offsetScrollSpeed);
                cameraOffset.m_Offset = new Vector3(cameraOffset.m_Offset.x, cameraOffset.m_Offset.y, zoomOffset);
            } else {
                zoomLevel += scrollDelta * offsetScrollSpeed;
                zoomLevel = Mathf.Clamp(zoomLevel, 0.2f, 1.0f);

                for (int i = 0; i < freeLook.m_Orbits.Length; i++) {
                    freeLook.m_Orbits[i] = new CinemachineFreeLook.Orbit {
                        m_Height = initOrbits[i].m_Height * zoomLevel,
                        m_Radius = initOrbits[i].m_Radius * zoomLevel
                    };
                }
            }
        }
    }
}
