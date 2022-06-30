using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackPanel : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler , IEndDragHandler    
{
    Vector2 firstPos = default;

    public static Action<int> OnRightSideDrag;
    public static Action OnRightSideTap;

    bool isDragging = default;    

    public void OnBeginDrag(PointerEventData eventData)
    {        
        isDragging = true;

        firstPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) { return; }

    public void OnEndDrag(PointerEventData eventData)
    {        
        isDragging = false;

        Vector2 delta = eventData.position - firstPos;        

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) return;

        if(delta.y > 0)        
            OnRightSideDrag?.Invoke(1);        
        else        
            OnRightSideDrag?.Invoke(-1);        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return;
        OnRightSideTap?.Invoke();
    }      
}
