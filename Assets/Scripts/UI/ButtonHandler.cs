using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonHandler : MonoBehaviour, IPointerDownHandler , IPointerUpHandler
{
    [SerializeField] UnityEvent PointerDownEvent;
    [SerializeField] UnityEvent PointerUpEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpEvent?.Invoke();
    }
}
