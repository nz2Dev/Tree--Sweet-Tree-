using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotationButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private bool holding;

    public bool IsHolding => holding;

    public void OnPointerDown(PointerEventData eventData) {
        holding = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        holding = false;
    }
}
